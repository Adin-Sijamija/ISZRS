using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class DolasciNaPosao
    {
        [Key]
        public int Id { get; set; }

        [Display(Name =("Redni broj sati"))]
        [Required(ErrorMessage ="Obavezno polje")]
        public int BrojSati { get; set; }
       

        [Display(Name =("Datum dolaska"))]
        public DateTime DatumDolaska { get; set; }
        [Display(Name = ("Datum Odlaska"))]
        public DateTime DatumOdlaska { get; set; }

        public int UgovorOraduId { get; set; }
        [ForeignKey("UgovorOraduId")]
        public Ugovori Ugovori { get; set; }



    }
}
