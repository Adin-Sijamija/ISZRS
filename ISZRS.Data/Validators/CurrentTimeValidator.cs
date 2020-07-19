using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ISZRS.Data.Validators
{
    class CurrentTimeValidator : ValidationAttribute
    {

        private DateTime trenutno { get; set; }

        public CurrentTimeValidator()
        {
            this.trenutno = DateTime.Now;
        }



        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (DateTime.Parse(value.ToString()) > DateTime.Now)
                return ValidationResult.Success;

            return new ValidationResult(GetErrorMessage());
        }

        private string GetErrorMessage()
        {
            return $"Rezervacije se mogu realizovati samo za buduce datume.";
        }

    }
}
