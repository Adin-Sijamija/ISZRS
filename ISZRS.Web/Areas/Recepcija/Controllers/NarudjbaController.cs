using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISZR.Web.Models;
using ISZRS.Data;
using ISZRS.Web.Areas.Recepcija.Data;
using ISZRS.Web.Areas.Recepcija.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ISZRS.Web.Areas.Recepcija.Controllers
{
    [Authorize]
    [Area("Recepcija")]
    public class NarudjbaController : Controller
    {
        private readonly MojContext db;

        public NarudjbaController(MojContext context)
        {
            db = context;
        }

        public IActionResult Index(int SobaZaduzenjeId)
        {
            ZaduzeneSobe soba = db.ZaduzeneSobe.Include(x => x.Sobe).SingleOrDefault(x => x.Id == SobaZaduzenjeId);
            NarudzbeSobeVM VM = new NarudzbeSobeVM
            {
                ZaduzeneSoba = soba
            };
            List<Narudzbe> narudzbe = db.Narudzbe.Where(x => x.ZaduzenaSobaId == soba.Id).ToList();
            foreach (var item in narudzbe)
            {
                List<NarudzbaHrana> NH = db.NarudzbaHrana.Include(x => x.Hrana).Where(x => x.NarudzbeID == item.Id).ToList();
                float cijena = 0;

                foreach (var NHrana in NH)
                {
                    cijena += NHrana.Hrana.Cijena * NHrana.Kolicina;
                }

                VM.Narudzbe.Add(new NarudzbeSobeVM.NarduzbaRed()
                {
                    Narudzba = item,
                    NarucenaHrana = NH,
                    ukupnaCijenaNarduzbe = cijena,
                    JeDostavljenja = item.DatumDostave < DateTime.Now ? true : false

                });

            }
            //ukloniti pogresne
            List<NarudzbeSobeVM.NarduzbaRed> pogresne = VM.Narudzbe.Where(x => x.ukupnaCijenaNarduzbe <= 0.0).ToList();
            VM.Narudzbe = VM.Narudzbe.Except(pogresne).ToList();
            List<Narudzbe> todelete = pogresne.Select(x => x.Narudzba).ToList();
            db.Narudzbe.RemoveRange(todelete);
            db.SaveChanges();

            foreach (var item in VM.Narudzbe)
            {
                VM.UkupnaCijena += item.ukupnaCijenaNarduzbe;
            }



            return View(VM);
        }


        public IActionResult NovaNaurzba(int SobaZaduzenjeId)
        {

            ZaduzeneSobe soba = db.ZaduzeneSobe.Include(x => x.Sobe).SingleOrDefault(x => x.Id == SobaZaduzenjeId);

            Narudzbe narudzbe = new Narudzbe();
            Narudzbe NovaNarudzba = narudzbe;
            NovaNarudzba.ZaduzenaSobaId = SobaZaduzenjeId;
            NovaNarudzba.DatumNarudzbe = DateTime.Now;
            db.Narudzbe.Add(NovaNarudzba);
            //test
            db.SaveChanges();

            return View(NovaNarudzba);

        }

     
        public async Task<IActionResult> PonistiNarudzbu(int narudzbaID, int SobaId)
        {

            db.Narudzbe.Remove(db.Narudzbe.Find(narudzbaID));
            await db.SaveChangesAsync();

            ZaduzeneSobe soba = await db.ZaduzeneSobe.Include(x => x.Rezervacija).SingleOrDefaultAsync(x => x.Id == SobaId);

            return RedirectToAction("Info", "Rezervacije", new { id = soba.RezervacijaId });

        }
        public IActionResult _Jela(string Naziv, int? CijenaOd, int? CijenaDo, int Stranica, int KartiPoStranici, int NarudzbaID)
        {


            List<Hrana> jela = db.Hrana.AsNoTracking().ToList();

            jela.OrderBy(x => x.Naziv);

            if (CijenaDo != null)
            {
                jela.RemoveAll(x => x.Cijena > CijenaDo);
            }
            if (CijenaOd != null)
            {
                jela.RemoveAll(x => x.Cijena < CijenaOd);
            }

            if (Naziv != String.Empty && Naziv != null)
            {
                jela = jela.Where(x => x.Naziv.Trim().ToLower().Contains(Naziv)).ToList();
            }



            jela.OrderByDescending(x => x.Cijena);

            ArrayList arrayList = new ArrayList(jela);
            Pagination pagination = new Pagination(arrayList, KartiPoStranici);

            var rezultat = pagination.GetStranicu(Stranica);

            arrayList = rezultat.Item1;
            List<Hrana> rezList = arrayList.Cast<Hrana>().ToList();

            if (rezultat.Item2 == true)
            {
                ViewData["ZadnjaStranica"] = "Svi artikli ispisani :D";
            }

            JelaVM VM = new JelaVM
            {
                Hrana = rezList,
                NarudzbaID = NarudzbaID
            };

            return View(VM);

        }



        [HttpPost]
        public IActionResult DodajJelo(int Kolicina, int NarudzbaID, int JeloID)
        {

            NarudzbaHrana PostojecaNarudzba = db.NarudzbaHrana.Where(x => x.HranaID == JeloID && x.NarudzbeID == NarudzbaID).SingleOrDefault();
            if (PostojecaNarudzba != null)
            {
                PostojecaNarudzba.Kolicina += Kolicina;
                db.NarudzbaHrana.Update(PostojecaNarudzba);
                db.SaveChanges();


            }
            else
            {

                NarudzbaHrana NovaNarudzbaHrana = new NarudzbaHrana
                {
                    HranaID = JeloID,
                    Kolicina = Kolicina,
                    NarudzbeID = NarudzbaID
                };

                db.NarudzbaHrana.Add(NovaNarudzbaHrana);
                db.SaveChanges();
            }


            Narudzbe Narudzba = db.Narudzbe.SingleOrDefault(x => x.Id == NarudzbaID);
            List<NarudzbaHrana> Jela = db.NarudzbaHrana.AsNoTracking().Include(x => x.Hrana).Where(x => x.NarudzbeID == Narudzba.Id).ToList();

            float cijena = 0;
            foreach (var jelo in Jela)
            {
                cijena += jelo.Kolicina * jelo.Hrana.Cijena;
            }
            Narudzba.UkupnaCijena = cijena;
            db.Narudzbe.Update(Narudzba);
            db.SaveChanges();

            int brojArtikala = 0;
            foreach (var item in Jela)
            {
                brojArtikala += item.Kolicina;
            }

            return Json(new { uspjeh = true, broj_artikala = brojArtikala });




        }

        public async Task<IActionResult> GetKorpaAsync(int id)
        {
            List<NarudzbaHrana> narudzbaHrana = await db.NarudzbaHrana.Include(x => x.Hrana).Where(x => x.NarudzbeID == id).ToListAsync();

            return PartialView("_NarudzbaKorpa", narudzbaHrana);
        }


        [HttpPost]
        public JsonResult Ukloni(int NarudznaHranaID)
        {
            NarudzbaHrana artikal = db.NarudzbaHrana.FirstOrDefault(x => x.Id == NarudznaHranaID);
            db.NarudzbaHrana.Remove(artikal);
            db.SaveChanges();

            Narudzbe narudzba = db.Narudzbe.SingleOrDefault(x => x.Id == artikal.NarudzbeID);
            int BrojArtikala = db.NarudzbaHrana.AsNoTracking().Where(x => x.NarudzbeID == narudzba.Id).Count();
            return Json(new { brojartikala = BrojArtikala });
        }

        public IActionResult ZavrsiNarudzbuGet(int narudzbaID)
        {
            ZavrsiNarudzbuModel VM = new ZavrsiNarudzbuModel
            {
                Narudzba = db.Narudzbe.Include(x => x.ZaduzenaSoba).SingleOrDefault(x => x.Id == narudzbaID)
            };
            VM.Rezervacija = db.Rezervacije.SingleOrDefault(x => x.RezervacijaId == VM.Narudzba.ZaduzenaSoba.RezervacijaId);
            VM.Artikli = db.NarudzbaHrana.Include(x => x.Hrana).Where(x => x.NarudzbeID == VM.Narudzba.Id).ToList();

            return View("ZavrsiNarudzbu", VM);
        }
        [HttpPost]
        public IActionResult ZavrsiNarudzbuPost(DateTime DatumDostave, DateTime VrijemeDostave, int narudzbaid)
        {
            Narudzbe narudzba = db.Narudzbe.SingleOrDefault(x => x.Id == narudzbaid);
            DateTime dateTime = new DateTime(DatumDostave.Year, DatumDostave.Month, DatumDostave.Day, VrijemeDostave.Hour, VrijemeDostave.Minute, VrijemeDostave.Second);
            DateTime Sad = DateTime.Now;

            if (Sad.Date == dateTime.Date)
            {
                if ((dateTime-Sad).Minutes<60)
                {
                    dateTime = dateTime.AddHours(1);
                    dateTime = dateTime.AddMinutes(5);
                }
              
            }


            narudzba.DatumDostave = dateTime;
            db.Update(narudzba);
            db.SaveChanges();

            return RedirectToAction("Index", new { SobaZaduzenjeId = narudzba.ZaduzenaSobaId });

            // return View();
        }

        public IActionResult PrekiniNarudzbu(int id)
        {
            Narudzbe narudzba = db.Narudzbe.FirstOrDefault(x => x.Id == id);
            int ZaduzenaOSbaId = narudzba.ZaduzenaSobaId;

            db.Narudzbe.Remove(narudzba);
            db.SaveChanges();

            return RedirectToAction("Index", new { SobaZaduzenjeId = ZaduzenaOSbaId });

        }

    }
}