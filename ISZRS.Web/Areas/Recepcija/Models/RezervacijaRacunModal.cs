using ISZR.Web.Models;
using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class RezervacijaRacunModal
    {
        public List<Rezervacije> rezervacija { get; set; }

        public List<SobeRow> Sobe { get; set; }

        public class SobeRow
        {
            public Sobe Soba { get; set; }
            public List<Namjestaj> Namjestaj { get; set; }
            public List<NarudzbeRow> NarudzbeHrana { get; set; }
        }

        public class NarudzbeRow
        {
            public Narudzbe Narudzba { get; set; }

            public List<NarudzbaHrana> NarudzbeHrana { get; set; }

        }

       //usluge

        
    }
}
