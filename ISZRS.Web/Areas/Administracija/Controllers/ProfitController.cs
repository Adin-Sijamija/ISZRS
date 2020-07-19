using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISZR.Web.Models;
using ISZRS.Data;
using ISZRS.Data.Models;
using ISZRS.Web.Areas.Administracija.Models;
using ISZRS.Web.Areas.Recepcija.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ISZRS.Web.Areas.Recepcija.Models.RacunNeGrupisanVM;

namespace ISZRS.Web.Areas.Administracija.Controllers
{
    [Area("Administracija")]
    [Authorize(Roles = ("Administrator"))]
    public class ProfitController : Controller
    {

        private readonly MojContext db;

        public ProfitController(MojContext db)
        {
            this.db = db;
        }

        public IActionResult Index(int? godina)
        {
            int Godina = 0;
            if (godina!=null)
            {
                Godina = (int)godina;
            }
            else
            {
                Godina = DateTime.Now.Year;
            }

            List<Rezervacije> rezervacije = db.Rezervacije.Where(x => x.DatumZavrsetkaRezerviranja.Year == Godina).ToList();

            GodisnjiProfitVM VM = new GodisnjiProfitVM()
            {
                Mjeseci = new List<GodisnjiProfitVM.ProfitMjesec>()
            };

            for (int i = 1; i < 13; i++)
            {
                GodisnjiProfitVM.ProfitMjesec Mjesec = new GodisnjiProfitVM.ProfitMjesec()
                {
                    Mjesec = i,
                    Racuni = new List<RacunNeGrupisanVM>()
                };

                foreach (var rez in rezervacije.Where(x=>x.DatumZavrsetkaRezerviranja.Month==i))
                {
                    RacunNeGrupisanVM Racun = RacunConstructor(rez.RezervacijaId);
                    Mjesec.Racuni.Add(Racun);

                }
                VM.Mjeseci.Add(Mjesec);
            }
           
            
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
    }
}