using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FinesRegister.Validators;

public class EstonianCarNumberAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var carNumber = value as string;

        if (carNumber == null || 
            (!Regex.IsMatch(carNumber, @"^\d{3}-[A-Z]{3}$") && !Regex.IsMatch(carNumber, @"^\d{2}-[A-Z]{3}$")))
        {
            return new ValidationResult("Sobimatu sõiduki numbri vorming. Aktsepteeritavad formaadid: 3 numbrit ja 3 tähte (näiteks 123-ABC) või 2 numbrit ja 3 tähte (näiteks 12-ABC)");
        }

        return ValidationResult.Success;
    }
}