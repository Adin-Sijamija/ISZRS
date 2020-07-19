using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class Narudzbe
    {
        [Key]
        public int Id { get; set; }
        public DateTime DatumNarudzbe { get; set; }
        public DateTime DatumDostave { get; set; }
        public float UkupnaCijena { get; set; }
        public int ZaduzenaSobaId { get; set; }
        [ForeignKey("ZaduzenaSobaId")]
        public ZaduzeneSobe ZaduzenaSoba { get; set; }
   
    }
}
