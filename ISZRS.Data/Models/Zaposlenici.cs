using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class Zaposlenici:Osobe
    {

        [StringLength(13)]
        [Required(ErrorMessage = "Obavezno Polje")]

        [RegularExpression("[0-9]{13}")]
        public string JMBG { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Obavezno Polje")]

        public string KorisnickoIme { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Obavezno Polje")]

        public string Sifra { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Obavezno Polje")]

        [Display(Name = "Broj telefona")]
        public string BrojTelefona { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Obavezno Polje")]

        public string Email { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Obavezno Polje")]

        [Display (Name = "Mjesto boravka")]
        public string MjestoBoravka { get; set; }
        [Display(Name = "Datum rođenja")]
        [DataType(DataType.Date)]
        public DateTime DatumRođenja { get; set; }
        

        public List<Zaduzivanja> Zaduzivanjas { get; set; }
        
        

    }
}
