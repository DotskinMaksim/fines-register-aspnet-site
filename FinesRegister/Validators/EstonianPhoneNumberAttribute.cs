using System.ComponentModel.DataAnnotations;

namespace FinesRegister.Validators;

public class EstonianPhoneNumberAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var phoneNumber = value as string;

        if (phoneNumber == null)
        {
            return new ValidationResult("Telefoninumber ei tohi olla tühi.");
        }

        if (phoneNumber.Length != 7 && phoneNumber.Length != 8)
        {
            return new ValidationResult("Kehtetu telefoninumbri vorming. Kehtiv vorming: 7 või 8 numbrit.");
        }

        if (!phoneNumber.All(char.IsDigit))
        {
            return new ValidationResult("Telefoninumber peab sisaldama ainult numbreid.");
        }

        return ValidationResult.Success;
    }
}