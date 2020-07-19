using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class GostaSoba
    {
        public int Id { get; set; }
        public int ZaduzenaSobaID { get; set; }
        [ForeignKey("ZaduzenaSobaID")]
        public ZaduzeneSobe ZaduzeneSobe { get; set; }
        public int GostiID { get; set; }
        [ForeignKey("GostiID")]
        public Gosti Gosti { get; set; }
    }
}
