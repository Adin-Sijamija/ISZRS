using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class Odjeli
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(40)]
        [Required]
        public string Naziv { get; set; }
        public int UgovorID { get; set; }
        [ForeignKey("UgovorID")]
        public Ugovori Ugovori { get; set; }
    }
}
