using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UslugeController : Controller
    {

        private readonly MojContext db;

        public UslugeController(MojContext context)
        {
            db = context;
        }

        public IActionResult Index(int rezId)
        {

            UslugeVM VM = new UslugeVM
            {
                Rezervacije = db.Rezervacije.FirstOrDefault(x => x.RezervacijaId == rezId)
            };

            List<Usluge> usluge = db.Usluge.ToList();


            VM.SlobodneUsluge = usluge;



            return View(VM);
        }

        public IActionResult ZaduzennjeInfo(int id)
        {
            return View();
        }
        public IActionResult PonistiUslugu(int ZaduzenjeID)
        {
            Zaduzivanja zaduzivanje = db.Zaduzivanja.SingleOrDefault(x => x.Id == ZaduzenjeID);

            List<UslugaDodaciZaduzenje> dodaci = db.UslugaDodaciZaduzenje.Where(x => x.ZaduzivanjaID == ZaduzenjeID).ToList();
            db.UslugaDodaciZaduzenje.RemoveRange(dodaci);


            db.Zaduzivanja.Remove(zaduzivanje);
            db.SaveChanges();

            return RedirectToAction("Index", new { rezId = zaduzivanje.RezervacijaId });
        }

        public IActionResult RezervacijaZaduzenja(int rezId)
        {
            List<Zaduzivanja> zaduzivanja = db.Zaduzivanja.Include(x => x.Rezervacije)
                .Include(x => x.Usluga).Where(x => x.RezervacijaId == rezId).OrderByDescending(x => x.KrajZaduzivanja).ToList();

            return View(zaduzivanja);
        }

        public IActionResult Info(int rezId, int uslugaId)
        {
            UslugaInfoVm VM = new UslugaInfoVm
            {
                Rezervacija = db.Rezervacije.FirstOrDefault(x => x.RezervacijaId == rezId),
                Usluga = db.Usluge.FirstOrDefault(x => x.Id == uslugaId)
            };

            List<Dodaci> Dodaci = db.Dodaci.Where(x => x.UslugeID == uslugaId).ToList();


            VM.Slodbodni = Dodaci;

            return View(VM);
        }

        public IActionResult DodajUsluguRezervacij(int rezId, int uslugaId)
        {

            UslugaRezervacijaDodavanjeVM VM = new UslugaRezervacijaDodavanjeVM
            {
                Rezervacija = db.Rezervacije.SingleOrDefault(x => x.RezervacijaId == rezId),
                Usluga = db.Usluge.SingleOrDefault(x => x.Id == uslugaId),
                Zaduzivanje = new Zaduzivanja()
            };

            return View(VM);


        }

        public IActionResult UrediDodatkeZaduzenja(int zaduzenjeId)
        {
            ZaduzenejInfoVM VM = ZaduzenjeInfoVMConstructor(zaduzenjeId);

            return View("ZaduzenjeDodaci", VM);
        }

        public IActionResult DodajDodatakZaduzenju(int DodatakId, int ZaduzenjeID)
        {
            Zaduzivanja zaduzivanje = db.Zaduzivanja.SingleOrDefault(x => x.Id == ZaduzenjeID);
            UslugaDodaciZaduzenje nova = new UslugaDodaciZaduzenje()
            {
                DodaciID = DodatakId,
                ZaduzivanjaID = zaduzivanje.Id,
                UslugaID = zaduzivanje.UslugaID,
                Kolicina = 1
            };

            db.UslugaDodaciZaduzenje.Add(nova);
            db.SaveChanges();

            UpdateRacun(ZaduzenjeID);

            return RedirectToAction("UrediDodatkeZaduzenja", new { zaduzenjeId = ZaduzenjeID });
        }


        public IActionResult UkloniDodataka(int DodatakZaduzenjaId, int ZaduzenjeID)
        {
            UslugaDodaciZaduzenje sgz = db.UslugaDodaciZaduzenje.SingleOrDefault(x => x.Id == DodatakZaduzenjaId);
            db.UslugaDodaciZaduzenje.Remove(sgz);
            db.SaveChanges();
            UpdateRacun(ZaduzenjeID);
            return RedirectToAction("UrediDodatkeZaduzenja", new { zaduzenjeId = ZaduzenjeID });

        }



        public IActionResult PromjeniKolicinuDodatka(int DodatakZaduzenjaId, int ZaduzenjeID, int opcija)
        {
            UslugaDodaciZaduzenje sgz = db.UslugaDodaciZaduzenje.SingleOrDefault(x => x.Id == DodatakZaduzenjaId);
            int BrojGostiju = db.GostiUsluga.Where(x => x.ZaduzivanjaID == ZaduzenjeID).Count();

            if (opcija == 0)
            {
                --sgz.Kolicina;
                if (sgz.Kolicina == 0)
                {
                    db.UslugaDodaciZaduzenje.Remove(sgz);
                    db.SaveChanges();
                    UpdateRacun(ZaduzenjeID);

                }
                else
                {
                    db.UslugaDodaciZaduzenje.Update(sgz);
                    db.SaveChanges();
                    UpdateRacun(ZaduzenjeID);

                }

            }
            if (opcija == 1)
            {
                ++sgz.Kolicina;

                if (sgz.Kolicina>BrojGostiju)
                {
                    return RedirectToAction("UrediDodatkeZaduzenja", new { zaduzenjeId = ZaduzenjeID });

                }

                db.UslugaDodaciZaduzenje.Update(sgz);
                db.SaveChanges();
                UpdateRacun(ZaduzenjeID);

            }

            return RedirectToAction("UrediDodatkeZaduzenja", new { zaduzenjeId = ZaduzenjeID });

        }

        public IActionResult DodajGosta(int ZaduzenjeID, int gostId)
        {
            GostiUsluga nova = new GostiUsluga()
            {
                GostiID = gostId,
                ZaduzivanjaID = ZaduzenjeID
            };
            db.GostiUsluga.Add(nova);
            db.SaveChanges();

            UpdateRacun(ZaduzenjeID);

            return RedirectToAction("ZaduzivanjeInfo", new { ZaduzenjeID });
        }

        public IActionResult UkloniGosta(int ZaduzenjeID, int gostId)
        {
            GostiUsluga nova = db.GostiUsluga.Single(x => x.GostiID == gostId && x.ZaduzivanjaID == ZaduzenjeID);
            db.GostiUsluga.Remove(nova);
            db.SaveChanges();

            UpdateRacun(ZaduzenjeID);


            return RedirectToAction("ZaduzivanjeInfo", new { ZaduzenjeID });

        }
        public IActionResult ZaduzivanjeInfo(int ZaduzenjeID)
        {
            ZaduzenejInfoVM VM = ZaduzenjeInfoVMConstructor(ZaduzenjeID);

            return View("VasaUslugaInfo", VM);
        }

        private ZaduzenejInfoVM ZaduzenjeInfoVMConstructor(int ZaduzenjeID)
        {
            ZaduzenejInfoVM VM = new ZaduzenejInfoVM
            {
                Zaduzenje = db.Zaduzivanja.
                Include(x => x.Rezervacije)
                .Include(x => x.Usluga)
                .SingleOrDefault(x => x.Id == ZaduzenjeID)
            };
            VM.Usluga = db.Usluge.SingleOrDefault(x => x.Id == VM.Zaduzenje.UslugaID);

            Rezervacije rez = db.Rezervacije
                .Include(x => x.ZaduzeneSobe)
                .ThenInclude(x => x.GostaSoba)
                .ThenInclude(x => x.Gosti)
                .SingleOrDefault(x => x.RezervacijaId == VM.Zaduzenje.RezervacijaId);


            List<Gosti> SviGosti = new List<Gosti>();
            List<Gosti> Slobodni = new List<Gosti>();
            List<Gosti> TrenutnoZaduzeni = new List<Gosti>();
            foreach (var ZS in rez.ZaduzeneSobe)
            {
                foreach (var GS in ZS.GostaSoba)
                {
                    SviGosti.Add(GS.Gosti);
                }
            }

            TrenutnoZaduzeni = db.GostiUsluga.Include(x => x.Gosti).Where(x => x.ZaduzivanjaID == ZaduzenjeID).Select(x => x.Gosti).ToList();
            Slobodni = SviGosti.Except(TrenutnoZaduzeni).ToList();

            VM.SlobodniGost = Slobodni;
            VM.ZaduzeniGosti = TrenutnoZaduzeni;

            List<UslugaDodaciZaduzenje> UDZ = db.UslugaDodaciZaduzenje
                .Include(x => x.Dodaci)
                .Where(x => x.ZaduzivanjaID == ZaduzenjeID && x.UslugaID == VM.Zaduzenje.UslugaID).ToList();
            VM.ZauzetiDodaci = UDZ;

            List<Dodaci> ZauzetiDodcai = UDZ.Select(x => x.Dodaci).ToList();

            List<Dodaci> TempDodaci = db.Dodaci.Where(x => x.UslugeID == VM.Zaduzenje.UslugaID).ToList();

            VM.SlobodniDodaci = TempDodaci.Except(ZauzetiDodcai).ToList();
            return VM;
        }

        [HttpPost]
        public IActionResult NovaUslugaDefault(int rezId, int uslugaId, DateTime DatumPocetka, DateTime DatumZavrsetka)
        {

            Usluge usluga = db.Usluge.SingleOrDefault(x => x.Id == uslugaId);

            Zaduzivanja zaduzivanje = new Zaduzivanja()
            {
                PocetakZaduzivanja = DatumPocetka,
                KrajZaduzivanja = DatumZavrsetka,
                RezervacijaId = rezId,
                UslugaID = uslugaId,
                JeZavršeno = false,
                UkupnaCijena = usluga.Cijena
            };

        


            db.Zaduzivanja.Add(zaduzivanje);
            db.SaveChanges();

            if (usluga.Cijena == 0)
            {
                List<Gosti> gostiRezervacije = db.GostaSoba.Include(x => x.ZaduzeneSobe).Include(x => x.Gosti).Where(x => x.ZaduzeneSobe.RezervacijaId == rezId).Select(x => x.Gosti).ToList();
                int brojGostiju = gostiRezervacije.Count();
                foreach (var item in gostiRezervacije)
                {
                    GostiUsluga nova = new GostiUsluga
                    {
                        GostiID = item.Id,
                        ZaduzivanjaID = zaduzivanje.Id
                    };

                    db.GostiUsluga.Add(nova);
                    db.SaveChanges();

                }

              DodajSveGoste(zaduzivanje.Id);
              DodajBesplatneDodatke(zaduzivanje.Id);

            }



            return RedirectToAction("ZaduzivanjeInfo", new { ZaduzenjeID = zaduzivanje.Id });
        }
        [HttpPost]

        public IActionResult NovaUslugaPoSatu(int rezId, int uslugaId, DateTime DatumPocetka, DateTime VrijemePocetka, int sati)
        {

            DateTime datumPocetkaZaduge = new DateTime(DatumPocetka.Year, DatumPocetka.Month, DatumPocetka.Day, VrijemePocetka.Hour, VrijemePocetka.Minute, VrijemePocetka.Second, VrijemePocetka.Millisecond);
            DateTime datumZavrsetkaZaduge = datumPocetkaZaduge.AddHours(sati);
            Usluge usluga = db.Usluge.SingleOrDefault(x => x.Id == uslugaId);

            Zaduzivanja zaduzivanje = new Zaduzivanja()
            {
                PocetakZaduzivanja = datumPocetkaZaduge,
                KrajZaduzivanja = datumZavrsetkaZaduge,
                RezervacijaId = rezId,
                UslugaID = uslugaId,
                JeZavršeno = false,
                UkupnaCijena = usluga.Cijena * sati
            };

            db.Zaduzivanja.Add(zaduzivanje);
            db.SaveChanges();

            DodajSveGoste(zaduzivanje.Id);
            DodajBesplatneDodatke(zaduzivanje.Id);

            return RedirectToAction("ZaduzivanjeInfo", new { ZaduzenjeID = zaduzivanje.Id });
        }
        [HttpPost]

        public IActionResult NovaUslugaDnevno(int rezId, int uslugaId, DateTime DatumPocetka, DateTime DatumZavrsetka)
        {

            Usluge usluga = db.Usluge.SingleOrDefault(x => x.Id == uslugaId);

            Zaduzivanja zaduzivanje = new Zaduzivanja()
            {
                PocetakZaduzivanja = DatumPocetka,
                KrajZaduzivanja = DatumZavrsetka,
                RezervacijaId = rezId,
                UslugaID = uslugaId,
                JeZavršeno = false,
                UkupnaCijena = ((DatumZavrsetka - DatumZavrsetka).Days) * usluga.Cijena
            };

            db.Zaduzivanja.Add(zaduzivanje);
            db.SaveChanges();

            
            return RedirectToAction("ZaduzivanjeInfo", new { ZaduzenjeID = zaduzivanje.Id });
        }


        public IActionResult NovaUslugaSedmicno(int rezId, int uslugaId, DateTime DatumPocetka, DateTime DatumZavrsetka)
        {

            Usluge usluga = db.Usluge.SingleOrDefault(x => x.Id == uslugaId);

            Zaduzivanja zaduzivanje = new Zaduzivanja()
            {
                PocetakZaduzivanja = DatumPocetka,
                KrajZaduzivanja = DatumZavrsetka,
                RezervacijaId = rezId,
                UslugaID = uslugaId,
                JeZavršeno = false,
                UkupnaCijena = ((DatumZavrsetka - DatumZavrsetka).Days) * usluga.Cijena
            };

            db.Zaduzivanja.Add(zaduzivanje);
            db.SaveChanges();


            return RedirectToAction("ZaduzivanjeInfo", new { ZaduzenjeID = zaduzivanje.Id });
        }



        public IActionResult NovoZaduzenje(int zaduzenjeID)
        {

            return View("DodajUsluguRezervacij", db.Zaduzivanja.Include(x => x.Rezervacije).Include(x => x.Usluga).FirstOrDefault(x => x.Id == zaduzenjeID));
        }

        [HttpPost]
        public IActionResult UrediZaduzenje(Zaduzivanja zaduzivanja)
        {


            if (ModelState.IsValid)
            {
                db.Zaduzivanja.Update(zaduzivanja);
                db.SaveChanges();
                return RedirectToAction("Index", "Pocetna");

            }



            return View("DodajUsluguRezervacij", db.Zaduzivanja.Include(x => x.Rezervacije).Include(x => x.Usluga).FirstOrDefault(x => x.Id == zaduzivanja.Id));
        }


        public void DodajBesplatneDodatke(int ZaduzenjeId)
        {
            Zaduzivanja zaduzenje = db.Zaduzivanja.Include(x => x.Usluga).SingleOrDefault(x => x.Id == ZaduzenjeId);
            List<Gosti> gostiRezervacije = db.GostaSoba.Include(x => x.ZaduzeneSobe).Include(x => x.Gosti).Where(x => x.ZaduzeneSobe.RezervacijaId == zaduzenje.RezervacijaId).Select(x => x.Gosti).ToList();
            int brojGostiju = gostiRezervacije.Count();

            List<Dodaci> dodaciBesplatni = db.Dodaci.Where(x => x.JeUkljucen && x.UslugeID == zaduzenje.UslugaID).ToList();

            foreach (var item in dodaciBesplatni)
            {

                UslugaDodaciZaduzenje novi = new UslugaDodaciZaduzenje()
                {
                    UslugaID = zaduzenje.UslugaID,
                    ZaduzivanjaID = zaduzenje.Id,
                    DodaciID = item.Id,
                    Kolicina = brojGostiju
                };
                db.UslugaDodaciZaduzenje.Add(novi);
                db.SaveChanges();

            }


        }

        public void DodajSveGoste(int ZaduzenjeId)
        {
            Zaduzivanja zaduzenje = db.Zaduzivanja.Include(x => x.Usluga).SingleOrDefault(x => x.Id == ZaduzenjeId);
            List<Gosti> gostiRezervacije = db.GostaSoba.Include(x => x.ZaduzeneSobe).Include(x => x.Gosti).Where(x => x.ZaduzeneSobe.RezervacijaId == zaduzenje.RezervacijaId).Select(x => x.Gosti).ToList();
            int brojGostiju = gostiRezervacije.Count();

            foreach (var item in gostiRezervacije)
            {
                GostiUsluga novi = new GostiUsluga()
                {
                    GostiID = item.Id,
                    ZaduzivanjaID = zaduzenje.Id
                };
                db.GostiUsluga.Add(novi);
                db.SaveChanges();

            }



        }


        public bool UpdateRacun(int ZaduzenjeId)
        {
            Zaduzivanja zaduzenje = db.Zaduzivanja.Include(x => x.Usluga).SingleOrDefault(x => x.Id == ZaduzenjeId);
            List<Gosti> gostiZaduzivanaj = db.GostiUsluga.Include(x => x.Gosti).Where(x => x.ZaduzivanjaID == ZaduzenjeId).Select(x => x.Gosti).ToList();
            List<UslugaDodaciZaduzenje> dodaci = db.UslugaDodaciZaduzenje.Include(x => x.Dodaci).Where(x => x.ZaduzivanjaID == ZaduzenjeId).ToList();


            int TipUsluge = (int)zaduzenje.Usluga.TipCijene;
            switch (TipUsluge)
            {
                case 0:
                    float cijena0 = 0;
                    foreach (var item in dodaci.Where(x => x.Dodaci.JeUkljucen == false))
                    {
                        cijena0 += item.Kolicina * item.Dodaci.Cijena;
                    }
                    zaduzenje.UkupnaCijena = cijena0;
                    db.Zaduzivanja.Update(zaduzenje);
                    db.SaveChanges();
                    break;

                case 1:
                    float cijena1 = 0;
                    int brojSati = (zaduzenje.KrajZaduzivanja - zaduzenje.PocetakZaduzivanja).Hours;
                    cijena1 = zaduzenje.Usluga.Cijena * brojSati;
                    foreach (var item in dodaci.Where(x => x.Dodaci.JeUkljucen == false))
                    {
                        cijena1 += item.Kolicina * item.Dodaci.Cijena;
                    }
                    zaduzenje.UkupnaCijena = cijena1;
                    db.Zaduzivanja.Update(zaduzenje);
                    db.SaveChanges();
                    break;


                case 2:
                    float cijena2 = 2;
                    cijena2 = zaduzenje.Usluga.Cijena * gostiZaduzivanaj.Count();
                    foreach (var item in dodaci.Where(x => x.Dodaci.JeUkljucen == false))
                    {
                        cijena2 += item.Kolicina * item.Dodaci.Cijena;
                    }
                    zaduzenje.UkupnaCijena = cijena2;
                    db.Zaduzivanja.Update(zaduzenje);
                    db.SaveChanges();
                    break;

                case 3:
                    float cijena3 = 0;
                    int brojdana = (zaduzenje.KrajZaduzivanja - zaduzenje.PocetakZaduzivanja).Days;
                    cijena3 = (zaduzenje.Usluga.Cijena *brojdana)* gostiZaduzivanaj.Count();
                    foreach (var item in dodaci.Where(x => x.Dodaci.JeUkljucen == false))
                    {
                        cijena3 += item.Kolicina * item.Dodaci.Cijena;
                    }
                    zaduzenje.UkupnaCijena = cijena3;
                    db.Zaduzivanja.Update(zaduzenje);
                    db.SaveChanges();
                    break;
                case 4:
                    float cijena4 = 0;
                    cijena4 = zaduzenje.Usluga.Cijena  * gostiZaduzivanaj.Count();
                    foreach (var item in dodaci.Where(x => x.Dodaci.JeUkljucen == false))
                    {
                        cijena4 += item.Kolicina * item.Dodaci.Cijena;
                    }
                    zaduzenje.UkupnaCijena = cijena4;
                    db.Zaduzivanja.Update(zaduzenje);
                    db.SaveChanges();
                    break;
                default:
                    break;
            }


            return true;
        }

    }
}