using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class RezervacijeIndexModal
    {
       
        public List<RezRow> RedRez { get; set; }

        public class RezRow
        {
            public Rezervacije Rezervacija { get; set; }
            public bool JeValidnaRezervacija { get; set; }
            public bool GlavniGostNedostaje { get; set; }
            public bool NemaSoba { get; set; }
            public bool ImaSlobodihSoba { get; set; }
            public bool NijeAktiviran { get; set; }
          //  public StatusRezervacije StusRezervacije { get; set; }
        }

      
    }
}
