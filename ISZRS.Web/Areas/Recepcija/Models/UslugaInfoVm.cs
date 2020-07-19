using ISZR.Web.Models;
using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class UslugaInfoVm
    {
        public Usluge Usluga { get; set; }
        public Rezervacije Rezervacija { get; set;}
        public Zaduzivanja Zaduzivanja { get; set; }
        public List<Dodaci> Podrazumjevani { get; set; }
        public List<Dodaci> Slodbodni { get; set; }
        public List<Dodaci> Zauzeti { get; set; }
    }
}
