using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class NamjestajSoba
    {
        public int Id { get; set; }
        public int SobeID { get; set; }
        [ForeignKey("SobeID")]
        public Sobe Sobe { get; set; }
        public int NamjestajID { get; set; }
        [ForeignKey("NamjestajID")]
        public Namjestaj Namjestaj { get; set; }

        public int? ZaduzenaSobaID { get; set; }
        [ForeignKey("ZaduzenaSobaID")]
        public ZaduzeneSobe ZaduzeneSobe { get; set; }
        public int Kolicina { get; set; }


    }
}
