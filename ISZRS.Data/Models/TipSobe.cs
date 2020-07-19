using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class TipSobe
    {
       
        public int Id { get; set; }

        [StringLength(30)]
        [Required(ErrorMessage = "Obavezno Polje")]
        public string Naziv { get; set; }
        [StringLength(100)]
        [Required(ErrorMessage = "Obavezno Polje")]
        public string Opis { get; set; }

        [Required(ErrorMessage = "Obavezno Polje")]
        [Display(Name ="Kapacitet gostiju")]
        public int Kapacitet { get; set; }

        public List<TipSobeNamjestaj> MoguciNamjestaj { get; set; }
    }
}
