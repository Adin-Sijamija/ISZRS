using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ISZRS.Data.Models
{
    public class Racun
    {
        public int Id { get; set; }

        public byte[] PdfDoc { get; set; }

        public DateTime datumKreacije { get; set; }

        public int? RezervacijaId { get; set; }
        [ForeignKey("RezervacijaId")]
        public Rezervacije Rezervacija { get; set; }

 

    }
}
