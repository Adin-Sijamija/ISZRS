using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class Gradovi
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Obavezno Polje")]
        public string Naziv { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage = "Obavezno Polje")]
        public string Regija { get; set; }
        public int DrzavaId { get; set; }
        [ForeignKey("DrzavaId")]
        public Drzave Drzave { get; set; }
    }
}
