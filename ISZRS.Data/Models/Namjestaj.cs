using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static ISZR.Web.Models.Usluge;

namespace ISZR.Web.Models
{
    public class Namjestaj
    {
        [Key]
        public int Id { get; set; }
        [StringLength(30)]
        [Required(ErrorMessage = "Obavezno Polje")]

        public string Ime { get; set; }
        [StringLength(100)]
        [Required(ErrorMessage = "Obavezno Polje")]

        public string Opis { get; set; }

        [DefaultValue(0)]
        public float Cijena { get; set; }
        [Display(Name ="Tip namještaja")]
        [EnumDataType(typeof(TipNamjestaja))]
        public TipNamjestaja tipNamjestaja { get; set; }
        [Display(Name ="Je Osnovni namještaj")]
        public bool JeOsnovniNamjestaj { get; set; }
        [Required(ErrorMessage = "Obavezno Polje")]

        public byte[] Slika { get; set; }

        public enum TipNamjestaja
        {
            [Description("Krevet")]
            Krevet = 0,

            [Description("Ormar")]
            Ormar = 1,

            [Description("Stol")]
            Stol = 2,

            [Description("Stolice")]
            Stolice =3,

            [Description("Kauč")]
            Kauc,

            [Description("Fotelja")]
            Fotelja,

            [Description("Ostalo")]
            Ostalo
           



        }
    }
}
