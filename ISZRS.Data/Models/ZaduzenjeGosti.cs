using ISZR.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ISZRS.Data.Models
{
    class ZaduzenjeGosti
    {
        [Key]
        public int Id { get; set; }
        public int ZaduzivanjaId { get; set; }
        [ForeignKey("ZaduzivanjaId")]
        public Zaduzivanja Zaduzivanja { get; set; }
        public int GostId { get; set; }
        [ForeignKey("GostId")]
        public Gosti Gost { get; set; }

    }
}
