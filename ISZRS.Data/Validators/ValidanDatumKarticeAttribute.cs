using ISZR.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace ISZRS.Data.Validators
{
    class ValidanDatumKarticeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(
             object value, ValidationContext validationContext)
        {
            KreditneKartice kartica = (KreditneKartice)validationContext.ObjectInstance;

            if (string.IsNullOrEmpty(kartica.VaziDo))
            {
                return new ValidationResult("Vazi do je obavezno polje ");
            }

            string VaziDo = kartica.VaziDo;

            //Regex regex = new Regex("@[0-9]{2}/[0-9]{2,4}");
            Regex regex = new Regex(@"^(0[1-9]|1[0-2])\/(([0-9]{4}|[0-9]{2})$)");
            Match match = regex.Match(VaziDo);
            if (!match.Success)
            {
                return new ValidationResult("Vazi do nije validan, prihvaćeni format je MM/GG ili MM/GGGG");

            }

            int mjesec = 0;
            int godina = 0;



            string[] datum = kartica.VaziDo.Split('/');

           

            if (datum[0].Length!=2)
            {
              
                return new ValidationResult("Mjesec nije validan");

            }
            if (datum[1].Length < 2 || datum[1].Length==3 || datum[1].Length>4)
            {
                
                return new ValidationResult("Godina nije validan");

            }

            mjesec = int.Parse(datum[0]);
            godina = int.Parse(datum[1]);


            if (mjesec > 12 || mjesec < 1)
            {
                return new ValidationResult("Mjesec nije validan");
            }
           

            if (godina < 100)
            {
                
                godina += 2000;

            }


            if (godina < DateTime.Now.Year)
            {
                return new ValidationResult("Godina nije validna, kreditna kratica je istekla");
            }

            DateTime sad = DateTime.Now.AddYears(25);
            if (godina>sad.Year)
            {
                return new ValidationResult("Godina nije validna, kreditna kratica traje predugo");

            }


            DateTime novi = new DateTime(godina, mjesec, 1);

            return ValidationResult.Success;

        }
    }

}