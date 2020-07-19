using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class Dodaci
    {
        [Key]
        public int Id { get; set; }
        [StringLength(30)]
        [Required(ErrorMessage = "Obavezno Polje")]
        public string Ime { get; set; }
        [StringLength(30)]
        [Required(ErrorMessage = "Obavezno Polje")]
        public string Opis { get; set; }

        public float Cijena { get; set; }
        [Display(Name ="Je uključen u osnovni paket")]
        public bool JeUkljucen { get; set; }
        [Required(ErrorMessage = "Obavezno Polje")]
        public byte[] Slika { get; set; }
        [Display(Name ="Dio usluge")]
        public int UslugeID { get; set; }
        [ForeignKey("UslugeID")]
        public Usluge Usluga { get; set; }

    }
}
