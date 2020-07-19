using ISZRS.Data.Models;
using ISZRS.Data.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ISZR.Web.Models
{
    public class Zaduzivanja
    {
        [Key]
        public int Id { get; set; }
       
        public int RezervacijaId { get; set; }
        [ForeignKey("RezervacijaId")]
        public Rezervacije Rezervacije { get; set; }
        public List<GostiUsluga> GostiUsluge { get; set; }
        public int UslugaID { get; set; }
        [ForeignKey("UslugaID")]
        public Usluge Usluga { get; set; }

        public DateTime PocetakZaduzivanja { get; set; }

        [UnutarVremenskogProstoraRezervacijeAttribute]
        public DateTime KrajZaduzivanja { get; set; }
        public bool JeZavršeno { get; set; }

        public float UkupnaCijena { get; set; }
       



    }
}
