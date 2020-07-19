using ISZR.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ISZR.Web.Models.Namjestaj;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class NajmjestajDodavanjeVM
    {

        public Sobe sobe { get; set;}
        public int ZaduzenjeID { get; set; }
        public TipNamjestaja tip { get; set; }
        public int RezID { get; set; }


        
    }
}
