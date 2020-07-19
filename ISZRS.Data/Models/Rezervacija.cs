using ISZR.Web.Models;
using ISZRS.Data.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ISZRS.Data.Models
{
    public class Rezervacije
    {
        [Key]
        public int RezervacijaId { get; set; }






        [Required(ErrorMessage = "Obavezno Polje")]

        [Display(Name = "Datum početka rezerviranja")]
        public DateTime DatumPocetkaRezerviranja { get; set; }
        [Required(ErrorMessage = "Obavezno Polje")]

        [Display(Name = "Datum završetka rezerviranja")]
        [ValidanDatumAttribute]
        public DateTime DatumZavrsetkaRezerviranja { get; set; }







        [Required(ErrorMessage = "Obavezno Polje")]

        [Display(Name = "Datum evidentiranja rezerviranja")]
        public DateTime DatumEvidentiranjaRezerviranja { get; set; }
        [Display(Name = "Rezervacija aktivna")]
        public bool RezervacijaAktivna { get; set; }
        [Display(Name = "Rezervacija završena")]
        public bool RezervacijaZavrsena { get; set; }

        public List<ZaduzeneSobe> ZaduzeneSobe { get; set; }
        public List<Zaduzivanja> Zaduzivanja { get; set; }


        public int? GlavniGostId { get; set; }
        [ForeignKey("GlavniGostId")]
        public Gosti GlavniGost { get; set; }
        [Display(Name ="Evidentirao Zaposlenik")]
        public string Zaposlenik { get; set; }
    
    }
}
