using System.ComponentModel.DataAnnotations;

namespace FinesRegister.Validators
{
    public class EstonianCVVAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var cvv = value as string;

           

            // CVV должен быть либо 3, либо 4 цифры
            if (cvv.Length < 3 || cvv.Length > 4)
            {
                return new ValidationResult("CVV-kood peab sisaldama 3 või 4 numbrit.");
            }

            // Проверка, что CVV состоит только из цифр
            if (!int.TryParse(cvv, out _))
            {
                return new ValidationResult("CVV-kood peab sisaldama ainult numbreid.");
            }

            return ValidationResult.Success;
        }
    }
}