using ISZRS.Web.Areas.Recepcija.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Administracija.Models
{
    public class GodisnjiProfitVM
    {
        public List<ProfitMjesec> Mjeseci { get; set; }
        

        public class ProfitMjesec
        {
            public int Mjesec { get; set; }
            public List<RacunNeGrupisanVM> Racuni { get; set; }
        }
    }
}
