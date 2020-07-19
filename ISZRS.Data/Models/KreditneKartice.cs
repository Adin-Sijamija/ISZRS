using ISZRS.Data.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class KreditneKartice
    {
        [Key]
        public int Id { get; set; }
        

        [Required(ErrorMessage ="Obavezno Polje")]
        [RegularExpression(@"[0-9]{2}\/[0-9]{2,4}",ErrorMessage = "Prihvaćeni format je MM/GG ili MM/GGGG")]
        [Display(Name ="Vazi Do")]
        [ValidanDatumKarticeAttribute]
        public string VaziDo { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Vazi Do")]
        public DateTime VaziDoDatum { get; set; }
        [MaxLength(20)]
        [Display(Name = "Broj Kartice")]
        [RegularExpression(@"[0-9]{13}",ErrorMessage = "Unesi te 13 cifri kreditne kartice")]
        [Required(ErrorMessage = "Obavezno Polje")]
        public string BrojKartice { get; set; }
        [MaxLength(5,ErrorMessage ="Maksimalan dužina polje je 5")]
        [Required(ErrorMessage = "Obavezno Polje")]
        [RegularExpression(@"[0-9]{3,5}", ErrorMessage = "Minimalno 3 broja")]
        public string CVV { get; set; }
        [Display(Name = "Tip Kartice")]
        public int TipKarticeID { get; set; }
        [ForeignKey("TipKarticeID")]
        public TipKartice TipKartice { get; set; }


    }
}
