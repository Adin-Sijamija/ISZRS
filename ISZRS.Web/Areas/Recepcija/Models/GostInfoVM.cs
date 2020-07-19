using ISZR.Web.Models;
using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class GostInfoVM
    {
        public Gosti Gost {get; set;}
        public ZaduzeneSobe GostovaSoba { get; set; }

        public KreditneKartice Kartica { get; set; }     
    
        public Rezervacije TrenutnaRezervacija { get; set; }

        public List<Rezervacije> PrijasnjeRezervacije { get; set; }
        public List<Rezervacije> BuduceRezervacije { get; set; }




    }
}
