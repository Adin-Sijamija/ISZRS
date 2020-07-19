using ISZR.Web.Models;
using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class ZavrsiNarudzbuModel
    {
        public Narudzbe Narudzba { get; set; }
        public Rezervacije Rezervacija { get; set; }

        public List<NarudzbaHrana> Artikli { get; set; }
    }
}
