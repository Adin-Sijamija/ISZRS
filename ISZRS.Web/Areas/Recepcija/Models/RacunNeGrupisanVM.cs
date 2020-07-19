using ISZR.Web.Models;
using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class RacunNeGrupisanVM
    {
        public Rezervacije Rezervacija { get; set; }
        public List<Gosti> GostiRezervacije { get; set; }

        public List<SobeCijena> SobeCijene { get; set; }    
        public float UkupnaCijenaSoba { get; set; }

        public List<NarudzbeRacun> NarudzbeRacuni { get; set; }
        public float UkupnaCijenaNarudzbi { get; set; }

        public List<NarudzbeRacun> NarudzbeRacuniZaPrekinut { get; set; }


        public List<UslugeRacun> UslugeRacuni { get; set; }
        public float UkupnaCijenaUsluga { get; set; }
        public List<UslugeRacun> UslugeRacuniZaPrekinut { get; set; }


        public float Total { get; set; }

        public class SobeCijena
        {
            public Sobe Soba { get; set; }
            public float CijenaSobe { get; set; }
            public List<SobeCijenaNamjestaj> sobeCijenaNamjestajs { get; set; }
            public float CijenaNamjestaja { get; set; }

            
        }

        public class SobeCijenaNamjestaj
        {
            public Namjestaj Namjestaj { get; set; }
            public int kolicina { get; set; }
            public float cijena { get; set; }
        }

        public class NarudzbeRacun
        {
            public Narudzbe Narudzba { get; set; }
            public List<NarudzbaHrana> Hrana { get; set; }
            public float CijenaNarudzbi { get; set; }

        }

        public class UslugeRacun
        {
            public Zaduzivanja Zaduzivanje { get; set; }
            public Usluge Usluga { get; set; }
            public List<UslugaDodaciZaduzenje> Dodaci { get; set; }
            public List<Gosti> Gosti { get; set; }
            public float cijena { get; set; }

        }

    }
}
