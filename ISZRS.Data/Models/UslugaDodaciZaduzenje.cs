using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class UslugaDodaciZaduzenje
    {   [Key]
        public int Id { get; set; }
        public int? UslugaID { get; set; }
        [ForeignKey("UslugaID")]
        public Usluge Usluge { get; set; }
        public int? DodaciID { get; set; }
        [ForeignKey("DodaciID")]
        public Dodaci Dodaci { get; set; }
        public int ZaduzivanjaID { get; set; }
        [ForeignKey("ZaduzivanjaID")]
        public Zaduzivanja Zaduzivanja { get; set; }

        public int Kolicina { get; set; }
    }
}
