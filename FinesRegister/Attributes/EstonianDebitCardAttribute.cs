using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FinesRegister.Attributes
{
    public class EstonianDebitCardAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var cardNumber = value as string;

            if (string.IsNullOrEmpty(cardNumber))
            {
                return new ValidationResult("Kaardi number on nõutav.");
            }

            if (cardNumber.Length != 16)
            {
                return new ValidationResult("Kaardi number peab olema 16 numbrit.");
            }

            if (!cardNumber.All(char.IsDigit))
            {
                return new ValidationResult("Kaardi number peab sisaldama ainult numbreid.");
            }

            string[] validPrefixes = { "5100", "5300", "5167" }; 
            bool isValidPrefix = validPrefixes.Any(prefix => cardNumber.StartsWith(prefix));

            if (!isValidPrefix)
            {
                return new ValidationResult("Kaardi number ei ole kehtiv.");
            }

            if (!IsValidLuhn(cardNumber))
            {
                return new ValidationResult("Kaardi number on kehtetu.");
            }

            return ValidationResult.Success;
        }

        private bool IsValidLuhn(string cardNumber)
        {
            int sum = 0;
            bool alternate = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(cardNumber[i].ToString());

                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                    {
                        n -= 9;
                    }
                }

                sum += n;
                alternate = !alternate;
            }

            return (sum % 10 == 0);
        }
    }
}
