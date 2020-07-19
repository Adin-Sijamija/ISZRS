using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class Ugovori
    {
        [Key]
        [Display(Name = "Broj Ugovora")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Obavezno Polje")]
        public DateTime DatumPocetkaUgovora { get; set; }


        [Required(ErrorMessage = "Obavezno Polje")]
        public DateTime DatumZavrsetkaUgovora { get; set; }

        [Required(ErrorMessage = "Obavezno Polje")]
        [Display(Name = "Broj radnih sati u mjesecu")]
        public int BrojRadnihSati { get; set; }

        [Display(Name = "Satnica zaposlenika")]
        [Required(ErrorMessage = "Obavezno polje")]
        public float Satnica { get; set; }
        [Required(ErrorMessage = "Obavezno polje")]
        [Display(Name = "Prekovremena satnica zaposlenika")]
        public float PrekovremenaSatnica { get; set; }

        //public int ZaposleniciID { get; set; }
        //[ForeignKey("ZaposleniciID")]
        //public Zaposlenici Zaposlenici { get; set; }

        [Display(Name = "Zaposlenik")]
        public string ZaposlenikKorisnickoIme { get; set; }

    }
}
