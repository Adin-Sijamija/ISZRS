using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ISZR.Web.Models;
using ISZRS.Data;
using ISZRS.Data.Models;
using ISZRS.Web.Areas.Recepcija.Models;
using jsreport.AspNetCore;
using jsreport.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Utilities;
using static ISZRS.Web.Areas.Recepcija.Models.RacunNeGrupisanVM;

namespace ISZRS.Web.Areas.Recepcija.Controllers
{
    [Authorize(Roles = "Recepcionar,Administrator")]
    [Area("Recepcija")]
    public class RacunController : Controller
    {
        private readonly MojContext db;
        private readonly IViewRenderService _viewRenderService;
        public IJsReportMVCService JsReportMVCService { get; }

        public RacunController(MojContext context, IViewRenderService viewRenderService, IJsReportMVCService jsReportMVCService)
        {
            db = context;
            _viewRenderService = viewRenderService;
            JsReportMVCService = jsReportMVCService;

        }




        public IActionResult Index(int rezId)
        {
            RacunNeGrupisanVM VM = RacunConstructor(rezId);

            return View(VM);

            //return View(db.Rezervacije.SingleOrDefault(x=>x.RezervacijaId==rezId));
        }


    

        public IActionResult GetRacun(int rezId)
        {
            RacunNeGrupisanVM VM = RacunConstructor(rezId);

            return View(VM);
        }

        private RacunNeGrupisanVM RacunConstructor(int rezId)
        {
            RacunNeGrupisanVM VM = new RacunNeGrupisanVM();
            VM.Rezervacija = db.Rezervacije.SingleOrDefault(x => x.RezervacijaId == rezId);

            VM.GostiRezervacije = db.GostaSoba
                .Include(x => x.ZaduzeneSobe)
                .ThenInclude(x => x.Rezervacija)
                .Include(x => x.Gosti)
                .ThenInclude(x => x.Gradovi)
                .ThenInclude(x => x.Drzave)
                .Where(x => x.ZaduzeneSobe.RezervacijaId == rezId).Select(x => x.Gosti).ToList();
            #region SobeCijenaProracun

            VM.SobeCijene = new List<SobeCijena>();
            VM.UkupnaCijenaSoba = 0;
            List<ZaduzeneSobe> Sobe = db.ZaduzeneSobe.Include(x => x.Sobe).ThenInclude(x => x.TipSobe).Where(x => x.RezervacijaId == rezId).ToList();

            foreach (var soba in Sobe)
            {
                SobeCijena novired = new SobeCijena
                {
                    sobeCijenaNamjestajs = new List<SobeCijenaNamjestaj>(),

                    Soba = soba.Sobe,
                    CijenaSobe = ((VM.Rezervacija.DatumZavrsetkaRezerviranja - VM.Rezervacija.DatumPocetkaRezerviranja).Days) * soba.Sobe.Cijena
                };
                VM.UkupnaCijenaSoba += novired.CijenaSobe;

                List<NamjestajSoba> namjestajSobe = db.NamjestajSoba
                    .Include(x => x.Namjestaj)
                    .Where(x => x.ZaduzenaSobaID == soba.Id && x.SobeID == soba.Sobe.Id).ToList();

                List<SobeCijenaNamjestaj> sobeCijenaNamjestaj = new List<SobeCijenaNamjestaj>();

                foreach (var item in namjestajSobe.Where(x => !x.Namjestaj.JeOsnovniNamjestaj))
                {
                    SobeCijenaNamjestaj novo = new SobeCijenaNamjestaj
                    {
                        Namjestaj = item.Namjestaj,
                        kolicina = item.Kolicina,
                        cijena = item.Kolicina * item.Namjestaj.Cijena
                    };
                    sobeCijenaNamjestaj.Add(novo);
                    VM.UkupnaCijenaSoba += novo.cijena;
                    novired.CijenaNamjestaja += novo.cijena;
                }
                novired.sobeCijenaNamjestajs = sobeCijenaNamjestaj;
                VM.SobeCijene.Add(novired);
            }

            #endregion

            #region NarudzbeRacun
            VM.NarudzbeRacuni = new List<NarudzbeRacun>();
            VM.UkupnaCijenaNarudzbi = 0;

            foreach (var item in Sobe)
            {
                List<Narudzbe> narudzbe = db.Narudzbe.Include(x => x.ZaduzenaSoba).ThenInclude(x => x.Sobe).Where(x => x.ZaduzenaSobaId == item.Id).ToList();

                foreach (var narudzba in narudzbe)
                {
                    NarudzbeRacun novi = new NarudzbeRacun
                    {
                        Hrana = new List<NarudzbaHrana>(),
                        Narudzba = narudzba
                    };
                    novi.Hrana = new List<NarudzbaHrana>();
                    novi.CijenaNarudzbi = 0;
                    List<NarudzbaHrana> hrana = db.NarudzbaHrana.Include(x => x.Hrana).Where(x => x.NarudzbeID == narudzba.Id).ToList();
                    foreach (var hran in hrana)
                    {
                        novi.Hrana.Add(hran);
                        novi.CijenaNarudzbi += hran.Kolicina * hran.Hrana.Cijena;

                    }
                    VM.NarudzbeRacuni.Add(novi);
                }


            }

            VM.NarudzbeRacuniZaPrekinut = VM.NarudzbeRacuni.Where(x => x.Narudzba.DatumDostave > DateTime.Now).ToList();
            VM.NarudzbeRacuni = VM.NarudzbeRacuni.Except(VM.NarudzbeRacuniZaPrekinut).ToList();
            foreach (var item in VM.NarudzbeRacuni)
            {
                VM.UkupnaCijenaNarudzbi += item.CijenaNarudzbi;
            }



            #endregion

            #region Usluge
            VM.UslugeRacuni = new List<UslugeRacun>();
            VM.UkupnaCijenaUsluga = 0;

            List<Zaduzivanja> Zaduzivanja = db.Zaduzivanja.Include(x => x.Usluga).Where(x => x.RezervacijaId == rezId).ToList();



            foreach (Zaduzivanja zaduzenej in Zaduzivanja)
            {
                UslugeRacun novi = new UslugeRacun
                {
                    Gosti = db.GostiUsluga.Include(x => x.Gosti).Where(x => x.ZaduzivanjaID == zaduzenej.Id).Select(x => x.Gosti).ToList(),


                    Zaduzivanje = zaduzenej,
                    cijena = zaduzenej.UkupnaCijena,
                    Dodaci = new List<UslugaDodaciZaduzenje>()
                };

                List<UslugaDodaciZaduzenje> dodaci = db.UslugaDodaciZaduzenje.Include(x => x.Dodaci).Where(x => x.UslugaID == zaduzenej.UslugaID && x.ZaduzivanjaID == zaduzenej.Id).ToList();

                foreach (var dodatak in dodaci)
                {
                    novi.Dodaci.Add(dodatak);

                }
                VM.UslugeRacuni.Add(novi);
            }

            VM.UslugeRacuniZaPrekinut = VM.UslugeRacuni.Where(x => x.Zaduzivanje.PocetakZaduzivanja > DateTime.Now).ToList();
            VM.UslugeRacuni = VM.UslugeRacuni.Except(VM.UslugeRacuniZaPrekinut).ToList();

            foreach (var item in VM.UslugeRacuni)
            {
                VM.UkupnaCijenaUsluga += item.cijena;
            }


            #endregion
            VM.Total = VM.UkupnaCijenaNarudzbi + VM.UkupnaCijenaSoba + VM.UkupnaCijenaUsluga;
            return VM;
        }

        [MiddlewareFilter(typeof(JsReportPipeline))]
        public IActionResult NapraviracunPDf(int rezId)
        {

            #region VM_Construkcija

            RacunNeGrupisanVM VM = new RacunNeGrupisanVM
            {
                Rezervacija = db.Rezervacije.SingleOrDefault(x => x.RezervacijaId == rezId),
                GostiRezervacije = db.GostaSoba
                 .Include(x => x.ZaduzeneSobe)
                 .ThenInclude(x => x.Rezervacija)
                 .Include(x => x.Gosti)
                 .ThenInclude(x => x.Gradovi)
                 .ThenInclude(x => x.Drzave)
                 .Where(x => x.ZaduzeneSobe.RezervacijaId == rezId).Select(x => x.Gosti).ToList()
            };



            List<GostaSoba> gostiSobe = db.GostaSoba
                   .Include(x => x.ZaduzeneSobe)
                   .ThenInclude(x => x.Sobe)
                   .ThenInclude(x => x.TipSobe)
                   .Where(x => x.ZaduzeneSobe.RezervacijaId == rezId).ToList();

            List<Gosti> gosti = new List<Gosti>();

            foreach (var gost in gostiSobe)
            {
                gosti.Add(db.Gosti.Include(x => x.Gradovi).ThenInclude(x => x.Drzave).SingleOrDefault(x => x.Id == gost.GostiID));
            }

            VM.GostiRezervacije = gosti;




            #region SobeCijenaProracun

            VM.SobeCijene = new List<SobeCijena>();
            VM.UkupnaCijenaSoba = 0;
            List<ZaduzeneSobe> Sobe = db.ZaduzeneSobe.Include(x => x.Sobe).ThenInclude(x => x.TipSobe).Where(x => x.RezervacijaId == rezId).ToList();

            foreach (var soba in Sobe)
            {
                SobeCijena novired = new SobeCijena
                {
                    sobeCijenaNamjestajs = new List<SobeCijenaNamjestaj>(),

                    Soba = soba.Sobe,
                    CijenaSobe = ((VM.Rezervacija.DatumZavrsetkaRezerviranja - VM.Rezervacija.DatumPocetkaRezerviranja).Days) * soba.Sobe.Cijena
                };
                VM.UkupnaCijenaSoba += novired.CijenaSobe;

                List<NamjestajSoba> namjestajSobe = db.NamjestajSoba
                    .Include(x => x.Namjestaj)
                    .Where(x => x.ZaduzenaSobaID == soba.Id && x.SobeID == soba.Sobe.Id).ToList();

                List<SobeCijenaNamjestaj> sobeCijenaNamjestaj = new List<SobeCijenaNamjestaj>();

                foreach (var item in namjestajSobe.Where(x => !x.Namjestaj.JeOsnovniNamjestaj))
                {
                    SobeCijenaNamjestaj novo = new SobeCijenaNamjestaj
                    {
                        Namjestaj = item.Namjestaj,
                        kolicina = item.Kolicina,
                        cijena = item.Kolicina * item.Namjestaj.Cijena
                    };
                    sobeCijenaNamjestaj.Add(novo);
                    VM.UkupnaCijenaSoba += novo.cijena;
                    novired.CijenaNamjestaja += novo.cijena;
                }
                novired.sobeCijenaNamjestajs = sobeCijenaNamjestaj;
                VM.SobeCijene.Add(novired);
            }

            #endregion

            #region NarudzbeRacun
            VM.NarudzbeRacuni = new List<NarudzbeRacun>();
            VM.UkupnaCijenaNarudzbi = 0;

            foreach (var item in Sobe)
            {
                List<Narudzbe> narudzbe = db.Narudzbe.Include(x => x.ZaduzenaSoba).ThenInclude(x => x.Sobe).Where(x => x.ZaduzenaSobaId == item.Id).ToList();

                foreach (var narudzba in narudzbe)
                {
                    NarudzbeRacun novi = new NarudzbeRacun
                    {
                        Hrana = new List<NarudzbaHrana>(),
                        Narudzba = narudzba
                    };
                    novi.Hrana = new List<NarudzbaHrana>();
                    novi.CijenaNarudzbi = 0;
                    List<NarudzbaHrana> hrana = db.NarudzbaHrana.Include(x => x.Hrana).Where(x => x.NarudzbeID == narudzba.Id).ToList();
                    foreach (var hran in hrana)
                    {
                        novi.Hrana.Add(hran);
                        novi.CijenaNarudzbi += hran.Kolicina * hran.Hrana.Cijena;

                    }
                    VM.NarudzbeRacuni.Add(novi);
                }


            }

            VM.NarudzbeRacuniZaPrekinut = VM.NarudzbeRacuni.Where(x => x.Narudzba.DatumDostave > DateTime.Now).ToList();
            VM.NarudzbeRacuni = VM.NarudzbeRacuni.Except(VM.NarudzbeRacuniZaPrekinut).ToList();
            foreach (var item in VM.NarudzbeRacuni)
            {
                VM.UkupnaCijenaNarudzbi += item.CijenaNarudzbi;
            }



            #endregion

            #region Usluge
            VM.UslugeRacuni = new List<UslugeRacun>();
            VM.UkupnaCijenaUsluga = 0;

            List<Zaduzivanja> Zaduzivanja = db.Zaduzivanja.Include(x => x.Usluga).Where(x => x.RezervacijaId == rezId).ToList();



            foreach (Zaduzivanja zaduzenej in Zaduzivanja)
            {
                UslugeRacun novi = new UslugeRacun
                {
                    Gosti = db.GostiUsluga.Include(x => x.Gosti).Where(x => x.ZaduzivanjaID == zaduzenej.Id).Select(x => x.Gosti).ToList(),


                    Zaduzivanje = zaduzenej,
                    cijena = zaduzenej.UkupnaCijena,
                    Dodaci = new List<UslugaDodaciZaduzenje>()
                };

                List<UslugaDodaciZaduzenje> dodaci = db.UslugaDodaciZaduzenje.Include(x => x.Dodaci).Where(x => x.UslugaID == zaduzenej.UslugaID && x.ZaduzivanjaID == zaduzenej.Id).ToList();

                foreach (var dodatak in dodaci)
                {
                    novi.Dodaci.Add(dodatak);

                }
                VM.UslugeRacuni.Add(novi);
            }

            VM.UslugeRacuniZaPrekinut = VM.UslugeRacuni.Where(x => x.Zaduzivanje.PocetakZaduzivanja > DateTime.Now).ToList();
            VM.UslugeRacuni = VM.UslugeRacuni.Except(VM.UslugeRacuniZaPrekinut).ToList();

            foreach (var item in VM.UslugeRacuni)
            {
                VM.UkupnaCijenaUsluga += item.cijena;
            }


            #endregion
            VM.Total = VM.UkupnaCijenaNarudzbi + VM.UkupnaCijenaSoba + VM.UkupnaCijenaUsluga;

            #endregion


            HttpContext.JsReportFeature().Recipe(Recipe.ChromePdf);

            string pdfName = "Rez" + rezId + "RacunPdf.pdf";
            //string stara = "report.pdf";
            Racun racun = new Racun();
            byte[] PdfByte;


            HttpContext.JsReportFeature().OnAfterRender((r) =>
            {
                using (var file = System.IO.File.Open(pdfName, FileMode.Create))
                {
                    r.Content.CopyTo(file);

                }
                r.Content.Seek(0, SeekOrigin.Begin);



                using (var PdfFile = System.IO.File.Open(pdfName, FileMode.Open))
                {
                    PdfByte = ReadFully(PdfFile);
                }
                racun.datumKreacije = DateTime.Now;
                racun.RezervacijaId = rezId;
                racun.PdfDoc = PdfByte;

                db.Racun.Add(racun);
                db.SaveChanges();

                VM.Rezervacija.RezervacijaAktivna = false;
                VM.Rezervacija.RezervacijaZavrsena = true;

                db.Rezervacije.Update(VM.Rezervacija);
                db.SaveChanges();



                string path = pdfName;
                FileInfo fi1 = new FileInfo(path);

                fi1.Delete();


            });

            return View("GetRacun", VM);

        }

        public IActionResult GetRacunPDf(int rezId)
        {
            Racun racun = db.Racun.Where(x => x.RezervacijaId == rezId).SingleOrDefault();

            return File(racun.PdfDoc, "application/pdf");
        }


        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }


    }

}