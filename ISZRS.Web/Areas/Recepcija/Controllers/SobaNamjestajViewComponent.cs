using ISZR.Web.Models;
using ISZRS.Data;
using ISZRS.Data.Models;
using ISZRS.Web.Areas.Recepcija.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Controllers
{
    public class SobaNamjestajViewComponent : ViewComponent
    {

        private readonly MojContext db;


        public SobaNamjestajViewComponent(MojContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke(int id,int ZaduzenejeID, Namjestaj.TipNamjestaja tip)
        {

            Sobe soba = db.Sobe.FirstOrDefault(x => x.Id == id);
            ZaduzeneSobe zaduzeneSoba = db.ZaduzeneSobe.FirstOrDefault(x => x.Id == ZaduzenejeID);

            List<Namjestaj> zaduzeni = db.NamjestajSoba.Include(x=>x.Namjestaj).Where(x => x.ZaduzenaSobaID == ZaduzenejeID && x.SobeID == id && x.Namjestaj.tipNamjestaja == tip).Select(x=>x.Namjestaj).ToList();

            List<TipSobeMoguciNamjestaj> Svi = db.TipSobeMoguciNamjestaj.Include(x => x.Namjestaj)
                .Where(x => x.TipSobeID == soba.TipSobeID).ToList();
            List<Namjestaj> svi = new List<Namjestaj>();
            foreach (var item in Svi.Where(x=>(int)x.Namjestaj.tipNamjestaja==(int)tip))
            {
                svi.Add(item.Namjestaj);
            }

            SobaNamjestajVM VM = new SobaNamjestajVM
            {
                ROWZaduzeniNamjestaj = new List<SobaNamjestajVM.ROW>()
            };
            foreach (var namj in zaduzeni)
            {
                VM.ROWZaduzeniNamjestaj.Add(new SobaNamjestajVM.ROW()
                {
                    Namjestaj = namj,
                    kolicina = db.NamjestajSoba.Where(x => x.SobeID == id
                    && x.NamjestajID == namj.Id &&x.ZaduzenaSobaID==ZaduzenejeID).Select(x=>x.Kolicina).FirstOrDefault()
                });
            }


            VM.ZaduzeneSoba = zaduzeneSoba;
            VM.SlobodniNamjestaj = svi;
            VM.ZaduzeniNamjestaj = zaduzeni;
            VM.SobaId = id;
            VM.TrenutniKapcitet =0;
            VM.UkupnaCijena = 0;

            foreach (var item in VM.ROWZaduzeniNamjestaj)
            {
                VM.TrenutniKapcitet += item.kolicina;
                VM.UkupnaCijena = item.kolicina*item.Namjestaj.Cijena;

            }



           
            VM.MaxKapcitet = db.TipSobeNamjestaj.Where(x => x.TipSobeID == soba.TipSobeID && x.TipNamjestaja == tip).Select(x => x.Kolicina).First();
            VM.Zaduzene = false;
            VM.TipSobe = tip;



            return View("SobaNamjestaj", VM);
        }

    }
}
