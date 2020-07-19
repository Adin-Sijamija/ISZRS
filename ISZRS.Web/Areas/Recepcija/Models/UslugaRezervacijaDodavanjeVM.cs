using ISZR.Web.Models;
using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class UslugaRezervacijaDodavanjeVM
    {
        public Rezervacije Rezervacija { get; set; }
        public Usluge Usluga { get; set; }
        public Zaduzivanja Zaduzivanje { get; set; }
    }
}
