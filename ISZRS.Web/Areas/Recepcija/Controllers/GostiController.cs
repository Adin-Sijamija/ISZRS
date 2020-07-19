using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISZR.Web.Models;
using ISZRS.Data;
using ISZRS.Data.Models;
using ISZRS.Web.Areas.Recepcija.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ISZRS.Web.Areas.Recepcija.Controllers
{
    [Area("Recepcija")]
    [Authorize]
    public class GostiController : Controller
    {
        private readonly MojContext db;

        public GostiController(MojContext context)
        {
            db = context;
        }

        public IActionResult DodajGoste(int id)
        {
            DodajGosteVM VM = DodajGosteVMConstructor(id);




            return View(VM);
        }


        private DodajGosteVM DodajGosteVMConstructor(int id)
        {
            DodajGosteVM VM = new DodajGosteVM
            {
                rezervacija = db.Rezervacije.SingleOrDefault(x => x.RezervacijaId == id),
                ZauzeteSobe = new List<DodajGosteVM.Row>()
            };

            List<ZaduzeneSobe> zaduzeneSobe = db.ZaduzeneSobe.Include(x => x.Sobe).ThenInclude(x => x.TipSobe).Where(x => x.RezervacijaId == id).ToList();

            foreach (var soba in zaduzeneSobe)
            {
                DodajGosteVM.Row Row = new DodajGosteVM.Row
                {
                    ZaduzenaSoba = soba
                };

                List<GostaSoba> gostiSobe = db.GostaSoba
                    .Include(x => x.ZaduzeneSobe)
                    .ThenInclude(x => x.Sobe)
                    .ThenInclude(x => x.TipSobe)
                    .Where(x => x.ZaduzenaSobaID == soba.Id).ToList();
                List<Gosti> gosti = new List<Gosti>();

                foreach (var gost in gostiSobe)
                {
                    gosti.Add(db.Gosti.Include(x => x.Gradovi).ThenInclude(x => x.Drzave).SingleOrDefault(x => x.Id == gost.GostiID));
                }

                Row.MaxKapacitet = soba.Sobe.TipSobe.Kapacitet;
                Row.TrenutniKapacitet = db.GostaSoba.Where(x => x.ZaduzenaSobaID == soba.Id).Count();

                Row.Gosti = gosti;


                VM.ZauzeteSobe.Add(Row);
            }

            return VM;
        }



        [HttpPost]
        public JsonResult DodajGostaUSobu(int rezId, int SobaId, int GostId)
        {
            GostaSoba NoviGost = new GostaSoba()
            {
                GostiID = GostId,
                ZaduzenaSobaID = SobaId
            };
            db.GostaSoba.Add(NoviGost);
            db.SaveChanges();


            return Json("Gost uspjenšo dodan u sobu!");
        }

        public JsonResult UkloniGosta(int rezId, int SobaId, int GostId)
        {
            GostaSoba ZaUklonit = db.GostaSoba.Where(x => x.ZaduzenaSobaID == SobaId && x.GostiID == GostId).SingleOrDefault();

            db.GostaSoba.Remove(ZaUklonit);
            db.SaveChanges();

            return Json("Gost uspjenšo uklonjen iz sobe! ----CONTROLLER");
        }

        public JsonResult ZamjeniGostovuSobu(int rezId, int SobaZaUklonitId, int SobaZaDodatId, int GostId)
        {
            GostaSoba ZaUklonit = db.GostaSoba.Where(x => x.ZaduzenaSobaID == SobaZaUklonitId && x.GostiID == GostId).SingleOrDefault();

            db.GostaSoba.Remove(ZaUklonit);
            db.SaveChanges();


            GostaSoba NoviGost = new GostaSoba()
            {
                GostiID = GostId,
                ZaduzenaSobaID = SobaZaDodatId
            };
            db.GostaSoba.Add(NoviGost);
            db.SaveChanges();


            return Json("Gost uspjenšo uklonjen iz sobe! i dodan u drugu sobu");

        }

        public IActionResult UpdatejtPanel(int rezID)
        {
            DodajGosteVM VM = DodajGosteVMConstructor(rezID);

            return PartialView("_SobePaneli", VM);
        }

        public IActionResult _SobePaneli(int ZaduzenaSobaId, int GostID, int rezID, bool SamoUkloni)
        {

            //Pronadzi gosta ako vec ga vec ima ukloni ga
            Rezervacije rez = db.Rezervacije.SingleOrDefault(x => x.RezervacijaId == rezID);

            List<ZaduzeneSobe> zaduzeneSobe = db.ZaduzeneSobe
                .Include(x => x.GostaSoba)
                .Where(x => x.RezervacijaId == rezID).ToList();

            List<GostaSoba> gostaSobe = new List<GostaSoba>();
            foreach (var zaduzenje in zaduzeneSobe)
            {
                foreach (var gostisoba in zaduzenje.GostaSoba)
                {
                    gostaSobe.Add(gostisoba);
                }
            }
            try
            {
                GostaSoba vecGost = gostaSobe.Find(x => x.GostiID == GostID);

                db.GostaSoba.Remove(vecGost);
                db.SaveChanges();
            }
            catch (Exception)
            {


            }

            if (SamoUkloni == false)
            {
                //dodaj ga u novi sobu
                GostaSoba nova = new GostaSoba()
                {
                    GostiID = GostID,
                    ZaduzenaSobaID = ZaduzenaSobaId
                };


                db.GostaSoba.Add(nova);
                db.SaveChanges();

            }



            DodajGosteVM VM = DodajGosteVMConstructor(rezID);

            //return PartialView(VM);
            return RedirectToAction("DodajGoste", "Gosti", new { id = rezID });
        }

        public IActionResult _GostiTabela(string ImePrezime, int RezId = 0)
        {
            if (RezId == 0)
            {
                return PartialView(null);

            }

            Rezervacije Rez = db.Rezervacije
                .Include(x => x.ZaduzeneSobe)
              .ThenInclude(x => x.GostaSoba)
              .ThenInclude(x => x.Gosti)
              .SingleOrDefault(x => x.RezervacijaId == RezId);

            List<Rezervacije> SveRez = db.Rezervacije
              .Include(x => x.ZaduzeneSobe)
              .Include(x => x.ZaduzeneSobe)
              .ThenInclude(x => x.GostaSoba)
              .ThenInclude(x => x.Gosti)
              .Where(x => x.RezervacijaZavrsena == false).ToList();

            List<Rezervacije> Podudarajuce = new List<Rezervacije>();

            ////ukloni rezervacije koje  smetaju
            //pocela prije zavrile poslije (veca rezervacija)
            Podudarajuce.AddRange(
                SveRez.Where(x =>
                (x.DatumPocetkaRezerviranja < Rez.DatumPocetkaRezerviranja)
                &&
                (x.DatumZavrsetkaRezerviranja > Rez.DatumZavrsetkaRezerviranja)
                ).ToList()
                );

            //pocele prije završile usred--ok
            Podudarajuce.AddRange(
                SveRez.Where(x =>
                (x.DatumPocetkaRezerviranja < Rez.DatumPocetkaRezerviranja)
                &&
                (x.DatumZavrsetkaRezerviranja < Rez.DatumZavrsetkaRezerviranja && x.DatumZavrsetkaRezerviranja > Rez.DatumPocetkaRezerviranja)
                ).ToList()
                );

            ////pocele usred završile kasnije       
            Podudarajuce.AddRange(
                SveRez.Where(x =>
                (x.DatumPocetkaRezerviranja > Rez.DatumPocetkaRezerviranja && x.DatumPocetkaRezerviranja < Rez.DatumZavrsetkaRezerviranja)
                && (x.DatumZavrsetkaRezerviranja > Rez.DatumZavrsetkaRezerviranja)
                ).ToList()
                );

            //pocela i završila usred (manja rezeraciaj)
            Podudarajuce.AddRange(
               SveRez.Where(x =>
               (x.DatumPocetkaRezerviranja > Rez.DatumPocetkaRezerviranja && x.DatumPocetkaRezerviranja < Rez.DatumZavrsetkaRezerviranja)
               &&
               (x.DatumZavrsetkaRezerviranja > Rez.DatumPocetkaRezerviranja && x.DatumZavrsetkaRezerviranja < Rez.DatumZavrsetkaRezerviranja)
               ).ToList()
               );

            List<Gosti> NemoguciGosti = new List<Gosti>();
            foreach (var rez in Podudarajuce)
            {
                foreach (var zs in rez.ZaduzeneSobe)
                {
                    foreach (var gs in zs.GostaSoba)
                    {
                        NemoguciGosti.Add(gs.Gosti);
                    }
                }
            }


            foreach (var zs in Rez.ZaduzeneSobe)
            {
                foreach (var gs in zs.GostaSoba)
                {
                    NemoguciGosti.Add(gs.Gosti);
                }
            }

            //ukloni sve trenutne goste
            List<Gosti> sviGosti = db.Gosti
                .Include(x => x.Gradovi)
                .ThenInclude(x => x.Drzave)
                .Except(NemoguciGosti).ToList();


            if (ImePrezime != null)
            {
                string imePrezime = ImePrezime;
                List<Gosti> prihvatljivi = sviGosti.Where(x => x.PunoIme.Trim().ToLower().Contains(imePrezime.Trim().ToLower())).OrderBy(x => x.Prezime).ToList();

                if (prihvatljivi != null)
                {
                    return PartialView(prihvatljivi);

                }
                else
                {
                    return PartialView();
                }
            }
            else
            {
                if (sviGosti.Count > 10)
                {
                    return PartialView(sviGosti.OrderBy(x => x.Prezime).Take(10));

                }
                else
                {
                    return PartialView(sviGosti.OrderBy(x => x.Prezime));
                }

            }


        }

        public IActionResult Info(int GostId)
        {
            GostInfoVM VM = new GostInfoVM();

            Rezervacije rezervacija = db.GostaSoba
                .Include(x => x.ZaduzeneSobe)
                .ThenInclude(x => x.Rezervacija)
                .ThenInclude(x => x.GlavniGost)
                .Include(x => x.Gosti)
                .Where(x => x.GostiID == GostId && x.ZaduzeneSobe.Rezervacija.RezervacijaAktivna)
                .Select(x => x.ZaduzeneSobe.Rezervacija).FirstOrDefault();

            //GostaSoba gost = db.GostaSoba
            //    .Include(x => x.ZaduzeneSobe)
            //    .ThenInclude(x => x.Rezervacija)
            //    .Include(x => x.Gosti).ThenInclude(x => x.KreditneKartica)
            //    .Include(x=>x.Gosti)
            //    .ThenInclude(x=>x.Gradovi)
            //    .ThenInclude(x=>x.Drzave)
            //    .Where(x => x.GostiID == GostId && x.ZaduzeneSobe.Rezervacija.RezervacijaAktivna)
            //    .FirstOrDefault();


            

            ZaduzeneSobe soba = db.GostaSoba
                .Include(x => x.ZaduzeneSobe)
                .ThenInclude(x => x.Sobe)
                .ThenInclude(x => x.TipSobe)
                .Include(x => x.Gosti)
                .Where(x => x.GostiID == GostId && x.ZaduzeneSobe.Rezervacija.RezervacijaAktivna)
                .Select(x => x.ZaduzeneSobe).FirstOrDefault();



            VM.Gost = db.Gosti.Include(x => x.Gradovi).ThenInclude(x => x.Drzave).Include(x => x.KreditneKartica).SingleOrDefault(x => x.Id == GostId);
            VM.Kartica = VM.Gost.KreditneKartica;
            VM.GostovaSoba = soba;

            VM.TrenutnaRezervacija = rezervacija;
            VM.BuduceRezervacije = db.GostaSoba
              .Include(x => x.ZaduzeneSobe)
              .ThenInclude(x => x.Rezervacija)
              .ThenInclude(x => x.GlavniGost)
              .Include(x => x.Gosti)
              .Where(x => x.GostiID == GostId && x.ZaduzeneSobe.Rezervacija.RezervacijaAktivna == false && x.ZaduzeneSobe.Rezervacija.RezervacijaZavrsena == false)
              .Select(x => x.ZaduzeneSobe.Rezervacija).ToList();

            VM.PrijasnjeRezervacije = db.GostaSoba
            .Include(x => x.ZaduzeneSobe)
            .ThenInclude(x => x.Rezervacija)
            .ThenInclude(x => x.GlavniGost)
            .Include(x => x.Gosti)
            .Where(x => x.GostiID == GostId && x.ZaduzeneSobe.Rezervacija.RezervacijaAktivna == false && x.ZaduzeneSobe.Rezervacija.RezervacijaZavrsena == true)
            .Select(x => x.ZaduzeneSobe.Rezervacija).ToList();


            return View(VM);

        }

        public IActionResult _NoviGost()
        {
            var model = new Gosti();

            SelectList nova = new SelectList(db.Drzave, "Id", "Naziv");
            nova.Prepend(new SelectListItem()
            {
                Text = "Odaberi Drzavu",
                Value = "-1"
            });


            ViewData["DrazavaID"] = nova;


            return PartialView("_NoviGost", model);
        }

        public JsonResult GetGradoveDrzave(int DrzavaID)
        {
            List<Gradovi> gradovi = db.Gradovi.Where(x => x.DrzavaId == DrzavaID).ToList();


            var Gradovi = (from G in gradovi
                           select new { G.Id, G.Naziv });



            return Json(Gradovi);

        }

        [HttpPost]
        public IActionResult _NoviGost(Gosti gost)
        {

            //if (String.IsNullOrEmpty(gost.Adresa))
            //{
            //    ViewData["Adressa"] = "Molimo Izaberite Drzavu";
            //    return PartialView("_NoviGost", gost);

            //}

            if (ModelState.IsValid)
            {
              
                db.Gosti.Add(gost);
                db.SaveChanges();
                ViewData.ModelState.IsValid.ToString();
            }
            SelectList nova = new SelectList(db.Drzave, "Id", "Naziv");
            ViewData["DrazavaID"] = nova;


            return PartialView("_NoviGost", gost);
        }


        public IActionResult DodajGlavnogGosta(int rezID)
        {
            Rezervacije Rezervacija = db.Rezervacije.SingleOrDefault(x => x.RezervacijaId == rezID);

            List<Gosti> SviGosti = new List<Gosti>();

            SviGosti = db.GostaSoba
                .Include(x => x.ZaduzeneSobe).ThenInclude(x => x.Rezervacija)
                .Include(x=>x.Gosti).ThenInclude(x=>x.Gradovi).ThenInclude(x=>x.Drzave)
                .Where(x => x.ZaduzeneSobe.RezervacijaId == rezID)
                .Select(x => x.Gosti).ToList();

            ViewData["RezID"] = rezID;

            return View(SviGosti);
        }

        public IActionResult GetKreditnuKarticu(int GostId, int RezID)
        {
            Gosti gost = db.Gosti.Include(x => x.KreditneKartica).FirstOrDefault(x => x.Id == GostId);



            ViewData["TipKarticeID"] = new SelectList(db.TipKartice, "Id", "Naziv");
            ViewData["GostID"] = GostId;
            ViewData["RezIDD"] = RezID;


            if (gost.KreditneKartica != null)
            {
                return PartialView("KreditnaKartica", gost.KreditneKartica);

            }
            KreditneKartice nova = new KreditneKartice();
            return PartialView("KreditnaKartica", null);
        }

        [HttpPost]
        public IActionResult _DodajKarticu([Bind("VaziDo,BrojKartice,CVV,TipKarticeID")] KreditneKartice kartica, int GostID, int RezID)
        {
            if (ModelState.IsValid)
            {

                int mjesec = 0;
                int godina = 0;


                string[] datum = kartica.VaziDo.Split('/');

                mjesec = int.Parse(datum[0]);
                godina = int.Parse(datum[1]);
                DateTime novi = new DateTime(godina, mjesec, 1);

                kartica.VaziDoDatum = novi;
                db.Add(kartica);
                db.SaveChanges();

                Gosti gost = db.Gosti.FirstOrDefault(x => x.Id == GostID);
                gost.KreditneKarticaId = kartica.Id;
                db.Gosti.Update(gost);
                db.SaveChanges();

                //RedirectToAction("Index", "Rezervacije");
                KreditneKartice uspjehKartica = db.KreditneKartice.Include(x => x.TipKartice).SingleOrDefault(x => x.Id == kartica.Id);
                ViewData["TipKarticeID"] = new SelectList(db.TipKartice, "Id", "Naziv");
                ViewData["GostID"] = GostID;
                ViewData["RezIDD"] = RezID;

                return PartialView("KreditnaKartica", uspjehKartica);

            }

        


            ViewData["TipKarticeID"] = new SelectList(db.TipKartice, "Id", "Naziv");
            ViewData["GostID"] = GostID;
            ViewData["RezIDD"] = RezID;




            return PartialView("KreditnaKartica", kartica);
        }

        public IActionResult DodajGlavnogGostaComplete(int? GostID, int? RezID)
        {

            if (GostID==null ||RezID==null)
            {
                return RedirectToAction("DodajGlavnogGosta", new { rezId = RezID });
            }

            Rezervacije rez = db.Rezervacije.FirstOrDefault(x => x.RezervacijaId == RezID);
            rez.GlavniGostId = GostID;
            db.Rezervacije.Update(rez);
            db.SaveChanges();



            List<Gosti> gosti = db.GostaSoba.Include(x => x.ZaduzeneSobe).Include(x => x.Gosti).Where(x => x.ZaduzeneSobe.RezervacijaId == RezID).Select(x => x.Gosti).ToList();
            //List<Zaduzivanja> besplatnazaduznjea=db.Zaduzivanja.Include(x=>x.)



            return RedirectToAction("Info", "Rezervacije", new { id = RezID });


        }
    }
}