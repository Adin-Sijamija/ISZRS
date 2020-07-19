using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class TipKartice
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Obavezno Polje")]

        public string Naziv { get; set; }
    }
}
