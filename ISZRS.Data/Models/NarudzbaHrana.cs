using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class NarudzbaHrana
    {
        public int Id { get; set; }
        public int NarudzbeID { get; set; }
        [ForeignKey("NarudzbeID")]
        public Narudzbe Narudzbe { get; set; }
        public int HranaID { get; set; }
        [ForeignKey("HranaID")]
        public Hrana Hrana { get; set; }
        public int Kolicina { get; set; }
    }
}
