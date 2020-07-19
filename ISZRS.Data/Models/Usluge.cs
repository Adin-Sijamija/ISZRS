using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class Usluge
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
        [Display(Name = "Tip Cijene")]

        public TipNaplate TipCijene { get; set; }
        [Display(Name ="Tip Usluge")]
        public TipUsluga TipUsluge { get; set; }

        public enum TipUsluga {
            Fitness=0,
            Sport=1,
            Relax=2,

        }

        public byte[] Slika { get; set; }

        public enum TipNaplate
        {
            [Description("Besplatno")]
            Besplatno = 0,

            [Description("Po Satu")] //gosti ne utjecu
            PoSatu =1,

            [Description("Jednom")] //po gostu
            Jednom = 2,

            [Description("Dnevno")] 
            Dnevno = 3,      
            
            [Description("Sedmicno")]
            Sedmicno = 4,
            

        }

        public int GetEnumCijene()
        {
            return (int)this.TipCijene;
        }

    }
}
