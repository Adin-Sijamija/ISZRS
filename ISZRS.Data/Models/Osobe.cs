using ISZR.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ISZRS.Data.Models
{
    public class Osobe
    {

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Obavezno Polje")]
        public string Ime { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Obavezno Polje")]
        public string Prezime { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Obavezno Polje")]
        public string Adresa { get; set; }

        [Display(Name = "Puno Ime")]
        public string PunoIme
        {
            get
            {
                return Ime + " " + Prezime;
            }
        }

    }
}
