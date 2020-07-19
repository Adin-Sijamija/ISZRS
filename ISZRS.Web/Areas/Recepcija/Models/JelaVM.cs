using ISZR.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class JelaVM
    {
        public List<Hrana> Hrana { get; set; }
        public int ZaduzenaSobaID { get; set; }

        public int NarudzbaID { get; set; }
        //public Narudzbe Narudzba { get; set; }
        
    }
}
