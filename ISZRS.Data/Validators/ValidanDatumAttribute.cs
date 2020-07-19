using ISZRS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ISZRS.Data.Validators
{
    class ValidanDatumAttribute : ValidationAttribute
    {


        protected override ValidationResult IsValid(
              object value, ValidationContext validationContext)
        {
            Rezervacije rez = (Rezervacije)validationContext.ObjectInstance;

            if (rez.DatumPocetkaRezerviranja < DateTime.Now.AddDays(-1))
            {
                return new ValidationResult("Datum početka ne smije biti manji od trenutnog datuma");
            }

            if (rez.DatumPocetkaRezerviranja > rez.DatumZavrsetkaRezerviranja)
            {
                return new ValidationResult("Datum početka ne smije biti veci od datuma završetka rezerviranja");
            }



            if ((rez.DatumZavrsetkaRezerviranja - rez.DatumPocetkaRezerviranja).TotalHours < 24)
            {
                return new ValidationResult("Rezervacija mora trajati najmanje 24 sata!");
            }

            return ValidationResult.Success;
        }


    }
}
