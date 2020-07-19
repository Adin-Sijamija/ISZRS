using ISZR.Web.Models;
using ISZRS.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class RezervacijaVM
    {
        public Rezervacije rezervacija { get; set; }
        public List<Sobe> zaduzeneSobe { get; set; }
        public List<Sobe> SlobodneSobe { get; set; }
        public List<ZaduzeneSobe> zaduzivanja { get; set; }
        public List<GostaSoba> gostsoba { get; set; }

        public List<SelectListItem> TipoviSoba { get; set;   }


        public float UkupnaCijena { get; set; }

        

        

        
    }
}
