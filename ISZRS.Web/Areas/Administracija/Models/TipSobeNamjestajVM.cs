using ISZR.Web.Models;
using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Administracija.Models
{
    public class TipSobeNamjestajVM
    {
        public TipSobe TipSobe { get; set; }
        public int TipNamjestaja { get; set; }
        public List<Namjestaj> slobodni { get; set; }
        public List<TipSobeMoguciNamjestaj> zauzeti { get; set; }
    }
}
