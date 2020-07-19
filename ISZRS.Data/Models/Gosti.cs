using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class Gosti: Osobe
    {

        public int? KreditneKarticaId { get; set; }
        [ForeignKey("KreditneKarticaId")]
        public KreditneKartice KreditneKartica { get; set; }

        public int? GradId { get; set; }
        [ForeignKey("GradId")]
        public Gradovi Gradovi { get; set; }

        //[DataType(DataType.PhoneNumber)]
        //[Required(ErrorMessage ="")]
        //public string Telefon { get; set; }
        
    }
}
