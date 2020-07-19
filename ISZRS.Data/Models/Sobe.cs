using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class Sobe
    {   [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Obavezno Polje")]
        public int? BrojSobe { get; set; }
        [Required(ErrorMessage ="Obavezno Polje")]
        [Display(Name = "Broj sprata")]
        public int? BrojSprata { get; set; }
        [Required(ErrorMessage = "Obavezno Polje")]
        public float Cijena { get; set; }
        [Display(Name ="Tip sobe")]
        public int TipSobeID { get; set; }
        [ForeignKey("TipSobeID")]
        public TipSobe TipSobe { get; set; }

     

        public List<ZaduzeneSobe> ZaduzeneSobe { get; set; }
    }
}
