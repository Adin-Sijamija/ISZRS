using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class ZaduzeneSobe
    {

        [Key]
        public int Id { get; set; }
        public int SobaID { get; set; }
        [ForeignKey("SobaID")]
        public Sobe Sobe { get; set; }
        public int RezervacijaId { get; set; }
        [ForeignKey("RezervacijaId")]
        public Rezervacije Rezervacija { get; set; }
       

        public List<GostaSoba> GostaSoba { get; set; }
    }
}
