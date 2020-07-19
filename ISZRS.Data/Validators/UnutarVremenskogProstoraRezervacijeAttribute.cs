using ISZR.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ISZRS.Data.Validators
{
    public class UnutarVremenskogProstoraRezervacijeAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(
           object value, ValidationContext validationContext)
        {
            Zaduzivanja zad = (Zaduzivanja)validationContext.ObjectInstance;
            

            if (zad.PocetakZaduzivanja < DateTime.Now)
            {
                return new ValidationResult("Datum početka ne smije biti manji od trenutnog datuma");
            }

            if (zad.PocetakZaduzivanja > zad.Rezervacije.DatumZavrsetkaRezerviranja)
            {
                return new ValidationResult("Datum početka ne smije biti veći od datuma završetka rezerviranja");
            }

            if (zad.PocetakZaduzivanja > zad.KrajZaduzivanja)
            {
                return new ValidationResult("Datum početka ne smije biti veći od datuma završetka usluge");
            }


            if ((zad.PocetakZaduzivanja - zad.KrajZaduzivanja).Hours<1)
            {
                return new ValidationResult("Usluga mora trajati najmanje 1 sat");
            }

            
            int TipCijene = (int)zad.Usluga.TipCijene;
            switch (TipCijene)
            {
                default:
                    break;
                case 3:
                    if ((zad.PocetakZaduzivanja - zad.KrajZaduzivanja).Hours<5  )
                    {
                        return new ValidationResult("Usluga mora trajati najmanje 1 sat");

                    }
                    break;
                case 4:
                    if ((zad.PocetakZaduzivanja - zad.KrajZaduzivanja).Days < 7)
                    {
                        return new ValidationResult("Usluga T");

                    }
                    break;
            }



            return ValidationResult.Success;
        }

    }
}
