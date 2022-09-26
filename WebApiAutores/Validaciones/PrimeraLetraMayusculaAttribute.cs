using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Validaciones
{
    public class PrimeraLetraMayusculaAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || String.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            string PrimeraLetra = value.ToString()[0].ToString();
            if (!PrimeraLetra.Equals(PrimeraLetra.ToUpper()))
            {
                return new ValidationResult("La primera letra debe de ser mayúscula");
            }

            return ValidationResult.Success;
        }
    }
}
