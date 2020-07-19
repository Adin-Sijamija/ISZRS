using ISZR.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ISZRS.Data.Models
{
    public class GostiUsluga
    {

        public int Id { get; set; }
        public int ZaduzivanjaID { get; set; }
        [ForeignKey("ZaduzivanjaID")]
        public Zaduzivanja Zaduzivanja { get; set; }
        public int GostiID { get; set; }
        [ForeignKey("GostiID")]
        public Gosti Gosti { get; set; }

    }
}
