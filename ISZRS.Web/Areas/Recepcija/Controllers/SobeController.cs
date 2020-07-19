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
using Microsoft.EntityFrameworkCore;

namespace ISZRS.Web.Areas.Recepcija.Controllers
{
    [Authorize]
    [Area("Recepcija")]
    public class SobeController : Controller
    {
        private readonly MojContext db;


        public SobeController(MojContext context)
        {
            db = context;
        }

        public IActionResult Index(int id)
        {
            Rezervacije rez = db.Rezervacije
                .Include(x => x.ZaduzeneSobe)
                .ThenInclude(x => x.Sobe)
                .ThenInclude(x=>x.TipSobe)
                .Include(x => x.ZaduzeneSobe)
                .ThenInclude(x => x.GostaSoba)
                .SingleOrDefault(x => x.RezervacijaId == id);

            SobeURezervacijList VM = new SobeURezervacijList
            {
                Rezervacija = rez,
                zauzetaSoba = new List<SobeURezervacijList.ZauzetaSoba>()
            };

            foreach (var item in rez.ZaduzeneSobe)
            {
                VM.zauzetaSoba.Add(new SobeURezervacijList.ZauzetaSoba()
                {
                    ZaduzeneSoba = item,
                    soba = item.Sobe
                });
            }

           
            return View(VM);


        }

        public IActionResult DodajNamjestaj(int SobaZaduzenjeId)
        {

            Sobe soba  = db.ZaduzeneSobe.Include(x=>x.Sobe).SingleOrDefault(x => x.Id == SobaZaduzenjeId).Sobe;
            NajmjestajDodavanjeVM VM = new NajmjestajDodavanjeVM
            {
                sobe = soba,
                ZaduzenjeID = SobaZaduzenjeId
            };



            return View(VM);
        }


        public IActionResult _NamjestajSlobodan(int id, Namjestaj.TipNamjestaja tipNamjestaja, bool zaduzene)
        {

            List<Namjestaj> savNamjestaj = db.Namjestaj.Where(X => X.tipNamjestaja == tipNamjestaja).ToList();
            List<Namjestaj> vecZaduzen = db.NamjestajSoba.Where(x => x.SobeID == id).Select(x => x.Namjestaj).ToList();

            return PartialView("_NamjestajSlobodan", savNamjestaj);
        }


        public IActionResult _SobaNamjestaj(int id, Namjestaj.TipNamjestaja tip, int ZaduzenaId)
        {

            return ViewComponent("SobaNamjestaj", new { id, ZaduzenejeID= ZaduzenaId, tip });

        }

        [HttpPost]
        public JsonResult DodajNamjestajSobi(int SobaId, int NamjestajID,int ZaduzenaId)
        {


            //provjeri moze li stati
            Sobe sobe = db.Sobe
                .Include(x => x.TipSobe)
                .ThenInclude(x => x.MoguciNamjestaj)
                .SingleOrDefault(x => x.Id == SobaId);

            Namjestaj namjestaj = db.Namjestaj.FirstOrDefault(x => x.Id == NamjestajID);

            TipSobeNamjestaj tip = db.TipSobeNamjestaj
                .FirstOrDefault(x => x.TipSobeID == sobe.TipSobeID 
                && x.TipNamjestaja == namjestaj.tipNamjestaja);

            List<NamjestajSoba> zaduzeni = db.NamjestajSoba.Where(x => x.SobeID == SobaId && 
            x.ZaduzenaSobaID==ZaduzenaId&&
            x.Namjestaj.tipNamjestaja == namjestaj.tipNamjestaja).Include(x => x.Namjestaj).Include(x=>x.Sobe).ToList();

            int Max = tip.Kolicina;
            int kapacitet = 0;
            foreach (var item in zaduzeni)
            {
                kapacitet += item.Kolicina;
            }

            if (kapacitet> Max)
            {
                return Json(new  { uspjeh = false, poruka = "Kapacitet dostignut" });

            }



            //provjeri jeli moguce 
            NamjestajSoba postojeci = db.NamjestajSoba
                .Where(x => x.SobeID == SobaId
                && x.NamjestajID == NamjestajID
                && x.ZaduzenaSobaID == ZaduzenaId
                ).FirstOrDefault();

            if (postojeci != null)
            {
                ++postojeci.Kolicina;
                db.NamjestajSoba.Update(postojeci);
                db.SaveChanges();
                return Json(new { uspjeh = true, poruka = "Uspješno dodan" });


            }


            NamjestajSoba nova = new NamjestajSoba()
            {
                SobeID = SobaId,
                NamjestajID = NamjestajID,
                ZaduzenaSobaID=ZaduzenaId,
                Kolicina = 1
            };

            db.NamjestajSoba.Add(nova);
            db.SaveChanges();

            Namjestaj nam = db.Namjestaj.SingleOrDefault(x => x.Id == NamjestajID);

            return Json(new  { uspjeh = true, poruka = "Uspješno dodan" });

        }

        public JsonResult UkloniNamjestajSobe(int SobaId, int NamjestajID, int ZaduzenaId)
        {
            NamjestajSoba postojeci = db.NamjestajSoba.Where(x => x.SobeID == SobaId
            &&x.ZaduzenaSobaID==ZaduzenaId
            && x.NamjestajID == NamjestajID).FirstOrDefault();


            if (postojeci != null)
            {

                if (postojeci.Kolicina>1)
                {
                    --postojeci.Kolicina;
                    db.NamjestajSoba.Update(postojeci);
                    db.SaveChanges();
                    return Json(new JsonResult(new {uspjeh=true,poruka="Uspješno uklonjen" }));
                }
                if (postojeci.Kolicina==1)
                {
                    db.NamjestajSoba.Remove(postojeci);
                    db.SaveChanges();
                    return Json(new JsonResult(new {uspjeh=true,poruka="Uspješno uklonjen" }));



                }



            }

            return Json(new JsonResult(new { uspjeh = false, poruka = "Došlo je do greške" }));


        }



    }





}