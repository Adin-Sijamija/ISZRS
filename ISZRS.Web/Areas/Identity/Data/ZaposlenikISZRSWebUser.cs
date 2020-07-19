using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ISZRS.Web.Areas.Identity.Data
{
    public class ZaposlenikISZRSWebUser : IdentityUser
    {

        public string Ime { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Obavezno Polje")]
        public string Prezime { get; set; }



        [StringLength(13)]
        [Required]
        [RegularExpression("[0-9]{13}",ErrorMessage ="JMBG nije validan")]
        public string JMBG { get; set; }
    

        [Display(Name = "Datum rođenja")]
        [DataType(DataType.Date)]
        public DateTime DatumRođenja { get; set; }

        public byte[] Slika { get; set; }






    }
}
