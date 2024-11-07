using System.ComponentModel.DataAnnotations;

namespace FinesRegister.Attributes
{
    public class EstonianCVVAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var cvv = value as string;

           

            if (cvv.Length < 3 || cvv.Length > 4)
            {
                return new ValidationResult("CVV-kood peab sisaldama 3 või 4 numbrit.");
            }

            if (!int.TryParse(cvv, out _))
            {
                return new ValidationResult("CVV-kood peab sisaldama ainult numbreid.");
            }

            return ValidationResult.Success;
        }
    }
}