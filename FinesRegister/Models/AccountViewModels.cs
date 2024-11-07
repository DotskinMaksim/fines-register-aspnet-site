using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using FinesRegister.Attributes;

namespace FinesRegister.Models
{

    public class PaymentViewModel
    {
        public List<Fine> Fines { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public int SelectedPaymentMethodId { get; set; }
        public float TotalAmount { get; set; }
    }

    public class AddPaymentMethodViewModel
    {
        [Required(ErrorMessage = "Kaardi omaniku nimi on kohustuslik")]
        public string OwnerName { get; set; }

        [Required(ErrorMessage = "Kaardi number on kohustuslik")]
        [CreditCard(ErrorMessage = "Sisestage kehtiv kaardi number")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Aegumiskuupäeva kuu on kohustuslik")]
        [Range(1, 12, ErrorMessage = "Sisestage kehtiv kuu")]
        public int ExpirationMonth { get; set; }

        [Required(ErrorMessage = "Aegumiskuupäeva aasta on kohustuslik")]
        [Range(2023, 2100, ErrorMessage = "Sisestage kehtiv aasta")]
        public int ExpirationYear { get; set; }

        [Required(ErrorMessage = "CVV on kohustuslik")]
        [Range(100, 999, ErrorMessage = "CVV peab olema kolmekohaline")]
        public int CvvCode { get; set; }
    }
    
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required] 
        [Display(Name = "Eesnimi")] 
        public string FirstName { get; set; }
        

        [Required]
        [Display(Name = "Perenimi")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} peab olema vähemalt {2} tähemärki pikk", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Parool")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Kinnita parool")]
        [Compare("Password", ErrorMessage = "Parool ja kinnitusparool ei ühti")]
        public string ConfirmPassword { get; set; }

        [Required]
        [EstonianPhoneNumber]
        [Display(Name = "Telefoni number")]
        public string PhoneNumber { get; set; }


        [Required]
        [EstonianCarNumber]
        [Display(Name = "Auto number")]
        public string CarNumber { get; set; }


    }
    public class ConfirmCodeViewModel
    {
        public string Code { get; set; }

    }


    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Parool")]
        public string Password { get; set; }
        
        [Display(Name = "Mäleta mind?")] 
        public bool RememberMe { get; set; }

    }
}