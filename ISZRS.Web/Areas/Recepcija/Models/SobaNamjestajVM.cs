using ISZR.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class SobaNamjestajVM
    {
        public ZaduzeneSobe ZaduzeneSoba { get; set; }
        public bool Zaduzene { get; set; }
        public int TrenutniKapcitet { get; set; }
        public int MaxKapcitet { get; set; }
        public List<Namjestaj> ZaduzeniNamjestaj { get; set; }
        public List<Namjestaj> SlobodniNamjestaj { get; set; }

        public Namjestaj.TipNamjestaja TipSobe { get; set; }
        public int SobaId { get; set; }

        public float UkupnaCijena { get; set; }

        public List<ROW> ROWZaduzeniNamjestaj { get; set; }

        public class ROW
        {
            public Namjestaj Namjestaj { get; set; }
            public int kolicina { get; set; }

        }
    }
}
