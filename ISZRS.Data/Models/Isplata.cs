using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class Isplata
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Obavezno Polje")]

        public float Iznos { get; set; }
        [Required(ErrorMessage = "Obavezno Polje")]

        public int Mjesec { get; set; }
        [Required(ErrorMessage = "Obavezno Polje")]

        public int Godina { get; set; }
        public int UgovoriID { get; set; }
        [ForeignKey("UgovoriID")]
        public Ugovori Ugovori { get; set; }


    }
}
