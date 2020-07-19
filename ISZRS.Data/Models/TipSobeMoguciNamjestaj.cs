using ISZR.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ISZRS.Data.Models
{
    public class TipSobeMoguciNamjestaj
    {
        public int Id { get; set; }

        public int NamjestajID { get; set; }
        [ForeignKey("NamjestajID")]
        public Namjestaj Namjestaj { get; set; }
        public int TipSobeID { get; set; }
        [ForeignKey("TipSobeID")]
        public TipSobe TipSobe { get; set; }

    }
}
