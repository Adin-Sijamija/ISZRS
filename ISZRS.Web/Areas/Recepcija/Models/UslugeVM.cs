using ISZR.Web.Models;
using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class UslugeVM
    {
        public Rezervacije Rezervacije { get; set; }
        public List<Usluge> SlobodneUsluge { get; set; }
        public List<Usluge> ZauzeteUsluge { get; set; }

    }
}
