using ISZR.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static ISZR.Web.Models.Namjestaj;

namespace ISZRS.Data.Models
{
    public class TipSobeNamjestaj
    {

        //Klasa soigurava da samo određen broj namjestaj je moguc u jedno sobi da npr jednosoba soba ima 8 kreveta 

        public int Id { get; set; }

        public int TipSobeID { get; set; }

        [ForeignKey("TipSobeID")]
        public TipSobe TipSobe { get; set; }

        public TipNamjestaja TipNamjestaja { get; set; }

        public int Kolicina { get; set; }
    }
}
