using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ISZRS.Data;
using ISZRS.Data.Models;
using ISZRS.Web.Areas.Recepcija.Models;
using ISZR.Web.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace ISZRS.Web.Areas.Recepcija.Controllers
{
    [Authorize]
    [Area("Recepcija")]
    public class RezervacijeController : Controller
    {
        private readonly MojContext db;

        public RezervacijeController(MojContext context)
        {
            db = context;
        }

        public IActionResult Index(DateTime? pocetak, DateTime? kraj)
        {
            List<Rezervacije> Rezervacijee = db.Rezervacije
                .Include(x => x.GlavniGost)
                .Include(x => x.ZaduzeneSobe)
                .ThenInclude(x => x.GostaSoba)
                .Include(x => x.ZaduzeneSobe)
                .ThenInclude(x => x.Sobe)
                .ThenInclude(x => x.TipSobe)
                .Where(x => x.RezervacijaZavrsena == false).OrderBy(x => x.DatumPocetkaRezerviranja).ToList();


            if (pocetak != null)
            {
                Rezervacijee = Rezervacijee.Except(Rezervacijee.Where(x => x.DatumPocetkaRezerviranja < pocetak).ToList()).ToList();
            }
            if (kraj != null)
            {
                Rezervacijee = Rezervacijee.Except(Rezervacijee.Where(x => x.DatumPocetkaRezerviranja < kraj).ToList()).ToList();
            }

            ViewData["pocetak"] = pocetak ?? new DateTime();
            ViewData["kraj"] = kraj ?? new DateTime();






            RezervacijeIndexModal VM = new RezervacijeIndexModal() { RedRez = new List<RezervacijeIndexModal.RezRow>() };

            foreach (var rez in Rezervacijee)
            {
                RezervacijeIndexModal.RezRow red = new RezervacijeIndexModal.RezRow
                {
                    Rezervacija = rez,
                    JeValidnaRezervacija = true
                };


                if (rez.GlavniGostId == null)
                {
                    red.GlavniGostNedostaje = true;
                }
                if (rez.ZaduzeneSobe.Count == 0)
                {
                    red.NemaSoba = true;
                }
                else
                {
                    int maxGostiju = 0, trenutnoGostiju = 0;

                    foreach (var item in rez.ZaduzeneSobe)
                    {
                        maxGostiju += item.Sobe.TipSobe.Kapacitet;
                        trenutnoGostiju += item.GostaSoba.Count();
                    }

                    if (maxGostiju > trenutnoGostiju)
                    {
                        red.ImaSlobodihSoba = true;
                    }
                }


                if (rez.DatumPocetkaRezerviranja < DateTime.Now)
                {
                    red.NijeAktiviran = true;
                }
                if (red.NemaSoba || red.GlavniGostNedostaje || red.ImaSlobodihSoba /*|| red.NijeAktiviran*/)
                {
                    red.JeValidnaRezervacija = false;
                }



                VM.RedRez.Add(red);
            }

            return View(VM);
        }

        public IActionResult PoništiRezervaciju(int rezId)
        {

            Rezervacije rezervacija = db.Rezervacije.SingleOrDefault(x => x.RezervacijaId == rezId);
            db.Rezervacije.Remove(rezervacija);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult NovaRezervacija() { return View(); }
        [HttpPost]
        public IActionResult NovaRezervacija(Rezervacije rez)
        {

            if (ModelState.IsValid)
            {
                Rezervacije Nova = new Rezervacije
                {
                    DatumEvidentiranjaRezerviranja = DateTime.Now,
                    DatumPocetkaRezerviranja = rez.DatumPocetkaRezerviranja,
                    DatumZavrsetkaRezerviranja = rez.DatumZavrsetkaRezerviranja,
                    RezervacijaAktivna = false,
                    RezervacijaZavrsena = false,
                    Zaposlenik = rez.Zaposlenik,
                    GlavniGostId = rez.GlavniGostId ?? null
                };

                db.Rezervacije.Add(Nova);
                db.SaveChanges();


                //dodaj besplatne usluge
                List<Usluge> Besplatne = db.Usluge.Where(x => x.Cijena == 0).ToList();
               
                foreach (var usluga in  Besplatne)
                {
                    Zaduzivanja nova = new Zaduzivanja()
                    {
                        RezervacijaId = Nova.RezervacijaId,
                        UslugaID = usluga.Id,
                        PocetakZaduzivanja = Nova.DatumPocetkaRezerviranja,
                        KrajZaduzivanja = Nova.DatumZavrsetkaRezerviranja,
                        UkupnaCijena = 0,
                        JeZavršeno = false

                    };

                    db.Zaduzivanja.Add(nova);
                    db.SaveChanges();

                    
                }



                return RedirectToAction("DodajSobe", "Rezervacije", new { id = Nova.RezervacijaId });
            }
            return View();

        }

        public async Task<IActionResult> _TrenutnoDostupneSobe(DateTime DatumPocetkaRezerviranja, DateTime DatumZavrsetkaRezerviranja)
        {

            if (DatumPocetkaRezerviranja < DateTime.Now.AddDays(-1) || DatumZavrsetkaRezerviranja < DateTime.Now || DatumZavrsetkaRezerviranja < DatumPocetkaRezerviranja)
            {
                return View(null);
            }

            List<Rezervacije> SveRez = db.Rezervacije
                .Include(x => x.ZaduzeneSobe)
                .ThenInclude(x => x.Sobe)
                .Include(x => x.ZaduzeneSobe)
                .ThenInclude(x => x.GostaSoba)
                .Where(x => x.RezervacijaZavrsena == false).ToList();

            List<Rezervacije> Podudarajuce = new List<Rezervacije>();

            ////ukloni rezervacije koje  smetaju
            //pocela prije zavrile poslije (veca rezervacija)
            Podudarajuce.AddRange(
                SveRez.Where(x =>
                (x.DatumPocetkaRezerviranja < DatumPocetkaRezerviranja)
                &&
                (x.DatumZavrsetkaRezerviranja > DatumZavrsetkaRezerviranja)
                ).ToList()
                );

            //pocele prije završile usred--ok
            Podudarajuce.AddRange(
                SveRez.Where(x =>
                (x.DatumPocetkaRezerviranja < DatumPocetkaRezerviranja)
                &&
                (x.DatumZavrsetkaRezerviranja < DatumZavrsetkaRezerviranja && x.DatumZavrsetkaRezerviranja > DatumPocetkaRezerviranja)
                ).ToList()
                );

            ////pocele usred završile kasnije       
            Podudarajuce.AddRange(
                SveRez.Where(x =>
                (x.DatumPocetkaRezerviranja > DatumPocetkaRezerviranja && x.DatumPocetkaRezerviranja < DatumZavrsetkaRezerviranja)
                && (x.DatumZavrsetkaRezerviranja > DatumZavrsetkaRezerviranja)
                ).ToList()
                );

            //pocela i završila usred (manja rezeraciaj)
            Podudarajuce.AddRange(
               SveRez.Where(x =>
               (x.DatumPocetkaRezerviranja > DatumPocetkaRezerviranja && x.DatumPocetkaRezerviranja < DatumZavrsetkaRezerviranja)
               &&
               (x.DatumZavrsetkaRezerviranja > DatumPocetkaRezerviranja && x.DatumZavrsetkaRezerviranja < DatumZavrsetkaRezerviranja)
               ).ToList()
               );



            List<Sobe> ZauzeteSobe = new List<Sobe>();

            foreach (var rez in Podudarajuce)
            {
                foreach (var zaduzenje in rez.ZaduzeneSobe)
                {
                    ZauzeteSobe.Add(zaduzenje.Sobe);
                }
            }

            List<Sobe> Sve = await db.Sobe.Include(x => x.TipSobe).ToListAsync();
            List<Sobe> SlobodneSobe = await db.Sobe.Include(x => x.TipSobe).Except(ZauzeteSobe).ToListAsync();
            List<TipSobe> TipoviSoba = await db.TipSobe.ToListAsync();

            _TrenutnoDostupneSobeVM VM = new _TrenutnoDostupneSobeVM
            {
                SobePoTipu = new List<SobePoTipovima>(),
                pocetak = DatumPocetkaRezerviranja,
                kraj = DatumZavrsetkaRezerviranja
            };


            foreach (var item in TipoviSoba)
            {
                //VM.SobePoTipu.Add(new SobePoTipovima()
                //{
                //    NazivSobe = item.Naziv,
                //    TrenutnoDostupno = SlobodneSobe.Where(x => x.TipSobeID == item.Id).Count(),
                //    MaximalniBroj = Sve.Where(x => x.TipSobeID == item.Id).Count()


                //});

                SobePoTipovima tip = new SobePoTipovima
                {
                    NazivSobe = item.Naziv,
                    TrenutnoDostupno = SlobodneSobe.Where(x => x.TipSobeID == item.Id).Count(),
                    MaximalniBroj = Sve.Where(x => x.TipSobeID == item.Id).Count()
                };

                VM.SobePoTipu.Add(tip);

            }

            return PartialView(VM);
        }

        public IActionResult IndexPrijasnje()
        {

            return View(db.Rezervacije.Include(x => x.GlavniGost).Where(x => x.RezervacijaAktivna == false && x.RezervacijaZavrsena == true).ToList());
        }



        public IActionResult DodajGlavnogGosta(int id)
        {
            Gosti GlavniGost = new Gosti();
            return View(GlavniGost);
        }

        [HttpPost]
        public string DodajGlavnogGosta(int id, Gosti Gost)
        {
            return "ID JE " + id + " GOST JE " + Gost.PunoIme;
        }


        public IActionResult Info(int id)
        {
            return View(db.Rezervacije.Include(x => x.GlavniGost).Single(x => x.RezervacijaId == id));
        }


        #region DodajSobe


        public IActionResult DodajSobe(int id)
        {
            RezervacijaVM VM = VMConstructor(id);
            List<SelectListItem> TipSoba = new List<SelectListItem>()
           {
               new SelectListItem()
               {
                   Value="0",
                   Text="Sve"
               }
           };

            TipSoba.AddRange(db.TipSobe.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Naziv
            }));


            VM.TipoviSoba = TipSoba;


            return View(VM);
        }

        private RezervacijaVM VMConstructor(int id)
        {
            RezervacijaVM VM = new RezervacijaVM
            {
                rezervacija = db.Rezervacije.Include(x => x.GlavniGost).Single(x => x.RezervacijaId == id)
            };

            List<Rezervacije> SveRez = db.Rezervacije
                .Include(x => x.ZaduzeneSobe)
                .ThenInclude(x => x.Sobe)     
                .Include(x => x.ZaduzeneSobe)
                .ThenInclude(x=>x.GostaSoba)
                .Where(x => x.RezervacijaZavrsena == false).ToList();

            List<Rezervacije> Podudarajuce = new List<Rezervacije>();

            ////ukloni rezervacije koje  smetaju
            //pocela prije zavrile poslije (veca rezervacija)
            Podudarajuce.AddRange(
                SveRez.Where(x =>
                (x.DatumPocetkaRezerviranja < VM.rezervacija.DatumPocetkaRezerviranja)
                &&
                (x.DatumZavrsetkaRezerviranja>VM.rezervacija.DatumZavrsetkaRezerviranja)
                ).ToList()
                );

            //pocele prije završile usred--ok
            Podudarajuce.AddRange(
                SveRez.Where(x => 
                (x.DatumPocetkaRezerviranja < VM.rezervacija.DatumPocetkaRezerviranja)
                && 
                (x.DatumZavrsetkaRezerviranja < VM.rezervacija.DatumZavrsetkaRezerviranja && x.DatumZavrsetkaRezerviranja>VM.rezervacija.DatumPocetkaRezerviranja)
                ).ToList()
                );

            ////pocele usred završile kasnije       
            Podudarajuce.AddRange(
                SveRez.Where(x => 
                (x.DatumPocetkaRezerviranja > VM.rezervacija.DatumPocetkaRezerviranja && x.DatumPocetkaRezerviranja < VM.rezervacija.DatumZavrsetkaRezerviranja)
                &&(x.DatumZavrsetkaRezerviranja>VM.rezervacija.DatumZavrsetkaRezerviranja)
                ).ToList()
                );

            //pocela i završila usred (manja rezeraciaj)
            Podudarajuce.AddRange(
               SveRez.Where(x =>
               (x.DatumPocetkaRezerviranja > VM.rezervacija.DatumPocetkaRezerviranja &&x.DatumPocetkaRezerviranja<VM.rezervacija.DatumZavrsetkaRezerviranja)
               &&
               (x.DatumZavrsetkaRezerviranja > VM.rezervacija.DatumPocetkaRezerviranja && x.DatumZavrsetkaRezerviranja < VM.rezervacija.DatumZavrsetkaRezerviranja)
               ).ToList()
               );


            List<Sobe> ZauzeteSobe = new List<Sobe>();
            foreach (var rez in Podudarajuce)
            {
                foreach (var item in rez.ZaduzeneSobe)
                {
                    ZauzeteSobe.Add(item.Sobe);
                }
            }


            List<Sobe> SlobodneSobe = db.Sobe.Include(x => x.TipSobe).ToList();
            SlobodneSobe = SlobodneSobe.Except(ZauzeteSobe).ToList();


            VM.SlobodneSobe = SlobodneSobe;


            List<ZaduzeneSobe> zaduzeneSobe = db.ZaduzeneSobe.Include(x=>x.Sobe).Include(x=>x.GostaSoba).Where(x => x.RezervacijaId == id).ToList();
            VM.UkupnaCijena = 0;
            int BrojDana = (VM.rezervacija.DatumZavrsetkaRezerviranja - VM.rezervacija.DatumPocetkaRezerviranja).Days;
            foreach (var zad in zaduzeneSobe)
            {
                VM.UkupnaCijena += (BrojDana * zad.Sobe.Cijena);
            }
            VM.zaduzivanja = zaduzeneSobe;

            VM.zaduzeneSobe = new List<Sobe>();

            foreach (var item in zaduzeneSobe)
            {
                VM.zaduzeneSobe.Add(item.Sobe);
            }
            VM.SlobodneSobe = SlobodneSobe.Except(VM.zaduzeneSobe).ToList();

            
            return VM;
        }


        #endregion

        #region _Sobe (Slobodne sobe)
        public PartialViewResult _Sobe(int rezId, int? MinCijena, int? MaxCijena, string TipSobe, int TrenutnaStrana)
        {

            RezervacijaVM VM = VMConstructor(rezId);

            List<Sobe> Filter = VM.SlobodneSobe;

            if (MaxCijena != null)
            {
                Filter.RemoveAll(x => x.Cijena > MaxCijena);
            }
            if (MinCijena != null)
            {
                Filter.RemoveAll(x => x.Cijena < MinCijena);
            }


            int TipSobeInt = Convert.ToInt32(TipSobe);

            if (TipSobeInt != 0)
            {
                Filter.RemoveAll(x => x.TipSobeID != TipSobeInt);

            }


            VM.SlobodneSobe = Pageination(TrenutnaStrana, Filter);

            return PartialView("_Sobe", VM);
        }

        private List<Sobe> Pageination(int trenutnaStranica, List<Sobe> SlobodneSobe)
        {

            List<Sobe> PageSobe = new List<Sobe>();

            int BrojSobaPoStranici = 5;

            int pocetniIndex = 0;

            if (trenutnaStranica == 1)
            {

                if (SlobodneSobe.Count > 5)
                {
                    PageSobe = SlobodneSobe.GetRange(pocetniIndex, BrojSobaPoStranici);
                    return PageSobe;

                }
                else
                {
                    PageSobe = SlobodneSobe;
                    TempData["ZadnjaStranica"] = "Zadnja stranica dostignuta";

                    return PageSobe;

                }



            }
            else
            {
                int max = SlobodneSobe.Count();
                pocetniIndex = (trenutnaStranica - 1) * BrojSobaPoStranici;

                if (max < pocetniIndex + BrojSobaPoStranici)
                {

                    //int zadnjaStranicaBrojRedova = ((pocetniIndex + BrojSobaPoStranici) - max) - 1;
                    //PageSobe = SlobodneSobe.GetRange(pocetniIndex, zadnjaStranicaBrojRedova);
                    PageSobe = SlobodneSobe.GetRange(pocetniIndex, max - pocetniIndex);
                    TempData["ZadnjaStranica"] = "Zadnja stranica dostignuta";
                    return PageSobe;

                }


                PageSobe = SlobodneSobe.GetRange(pocetniIndex, BrojSobaPoStranici);

            }

            return PageSobe;
        }

        [HttpPost]
        public string _DodajSobu(int sobaID, int rezId)
        {
            ZaduzeneSobe nova = new ZaduzeneSobe
            {
                SobaID = sobaID,
                RezervacijaId = rezId
            };

            db.ZaduzeneSobe.Add(nova);
            db.SaveChanges();

            ZaduzeneSobe NovoDodata = db.ZaduzeneSobe
                .Include(x=>x.Sobe)
                .ThenInclude(x=>x.TipSobe)
                .SingleOrDefault(x => x.Id == nova.Id);

            foreach(Namjestaj.TipNamjestaja vrstaNamjestaja in Enum.GetValues(typeof(Namjestaj.TipNamjestaja)))
            {
                Namjestaj besplatniNamjestaj = db.Namjestaj.Where(x => x.JeOsnovniNamjestaj && (int)x.tipNamjestaja == (int)vrstaNamjestaja).FirstOrDefault();

                if (besplatniNamjestaj!=null)
                {

                    NamjestajSoba namjestaj = new NamjestajSoba()
                    {
                        NamjestajID = besplatniNamjestaj.Id,
                        SobeID = NovoDodata.SobaID,
                        ZaduzenaSobaID = NovoDodata.Id,
                        Kolicina = db.TipSobeNamjestaj.Where(x => (int)x.TipNamjestaja == (int)besplatniNamjestaj.tipNamjestaja && x.TipSobeID == NovoDodata.Sobe.TipSobeID).First().Kolicina

                    };
                    db.NamjestajSoba.Add(namjestaj);
                    db.SaveChanges();

                }





            }


            return "Uspjesno rezervisana soba broj: " + sobaID + " /n Za rezervaciju broj: " + rezId;
        }
        #endregion

        #region _RezSobe

        public IActionResult _RezSobe(int rezId)
        {
            RezervacijaVM VM = VMConstructor(rezId);

            return PartialView(VM);
        }

        [HttpPost]
        public string _UkloniSobu(int rezId, int sobaID)
        {
            ZaduzeneSobe updateovana = db.ZaduzeneSobe
                .Where(x => x.RezervacijaId == rezId && x.SobaID == sobaID).SingleOrDefault();
            try
            {
                db.ZaduzeneSobe.Remove(updateovana);
                db.SaveChanges();
                return "Uspješno ste uklonili sobu";
            }
            catch (Exception e)
            {
                return "Došlo je do greške /n"+e.Message+" "+e.Source +e.InnerException;

            }

        }


        #endregion

        public IActionResult AktivirajRezervaciju(int id)
        {
            AktivirajRezervacijuVM VM = new AktivirajRezervacijuVM
            {
                Rezervacija = db.Rezervacije.Include(x => x.GlavniGost).SingleOrDefault(x => x.RezervacijaId == id)
            };
            VM.Sobe = db.ZaduzeneSobe.Include(x => x.GostaSoba).ThenInclude(x => x.Gosti).ThenInclude(x=>x.Gradovi).ThenInclude(x=>x.Drzave).Include(x => x.Sobe).ThenInclude(x => x.TipSobe)
                .Where(x => x.RezervacijaId == VM.Rezervacija.RezervacijaId).ToList();

            return View(VM);
        }

        public IActionResult AktivirajRezervacijuConfirm(int id)
        {
            Rezervacije rez = db.Rezervacije.FirstOrDefault(x => x.RezervacijaId == id);
            rez.RezervacijaAktivna = true;
            db.Rezervacije.Update(rez);
            db.SaveChanges();


            return RedirectToAction("Info", "Rezervacije", new { id });
        }





    }
}
