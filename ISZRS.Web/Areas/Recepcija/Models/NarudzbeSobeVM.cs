using ISZR.Web.Models;
using ISZRS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class NarudzbeSobeVM
    {
 
        public NarudzbeSobeVM()
        {
            UkupnaCijena = 0;
            Narudzbe = new List<NarduzbaRed>();
            ZaduzeneSoba = null;
        }
        public ZaduzeneSobe ZaduzeneSoba { get; set; }
        public float UkupnaCijena { get; set; }
        public List<NarduzbaRed> Narudzbe { get; set; }

        public class NarduzbaRed
        {
            public Narudzbe Narudzba { get; set; }
            public List<NarudzbaHrana> NarucenaHrana { get; set; }
            public float ukupnaCijenaNarduzbe { get; set; }
            public bool JeDostavljenja { get; set; }

        }
    }
}
