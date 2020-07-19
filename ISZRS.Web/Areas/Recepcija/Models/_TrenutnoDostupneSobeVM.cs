using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISZRS.Web.Areas.Recepcija.Models
{
    public class _TrenutnoDostupneSobeVM
    {
        public List<SobePoTipovima> SobePoTipu {get; set;}
        public DateTime pocetak { get; set; }
        public DateTime kraj { get; set; }

    }

    public class SobePoTipovima
    {
        public string NazivSobe { get; set; }
        public int TrenutnoDostupno  { get; set; }
        public int MaximalniBroj { get; set; }

       
    }
}
