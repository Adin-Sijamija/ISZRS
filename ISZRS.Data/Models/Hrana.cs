using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class Hrana
    {
        [Key]
        public int Id { get; set; }
        [StringLength(30)]
        [Required(ErrorMessage = "Obavezno Polje")]

        public string Naziv { get; set; }
        [StringLength(30)]
        [Required(ErrorMessage = "Obavezno Polje")]

        public string Opis { get; set; }

        [Required(ErrorMessage = "Obavezno Polje")]

        public float Cijena { get; set; }
        [Required(ErrorMessage ="Obavezno Polje")]
        public byte[] Slika { get; set; }








    }

}
