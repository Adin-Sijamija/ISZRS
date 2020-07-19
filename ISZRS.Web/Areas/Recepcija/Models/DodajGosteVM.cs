using ISZR.Web.Models;
using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class DodajGosteVM
    {
        public Rezervacije rezervacija { get; set; }

        public List<Row> ZauzeteSobe { get; set; }

        //random gosti da popune tabelu za sobe akd se ocita>
        public List<Gosti> SlobodniGosti { get; set; }
    

        public class Row
        {
            public ZaduzeneSobe ZaduzenaSoba { get; set; }
            public List<Gosti> Gosti { get; set; }
            public int MaxKapacitet { get; set; }
            public int TrenutniKapacitet { get; set; }

        }
    }
}
