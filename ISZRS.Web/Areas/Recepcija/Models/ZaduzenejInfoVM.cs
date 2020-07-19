using ISZR.Web.Models;
using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class ZaduzenejInfoVM
    {
        public Zaduzivanja Zaduzenje { get; set; }
        public Usluge Usluga { get; set; }
        public List<Dodaci> SlobodniDodaci { get; set; }
        public List<UslugaDodaciZaduzenje> ZauzetiDodaci { get; set; }
        public List<Gosti> SlobodniGost { get; set; }
        public List<Gosti> ZaduzeniGosti { get; set; }
    }
}
