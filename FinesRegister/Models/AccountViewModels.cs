using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using FinesRegister.Attributes;

namespace FinesRegister.Models
{

    public class PaymentMethodAddViewModel
    {
        
        [Required(ErrorMessage = "Omaniku nimi on nõutav")]
        [Display(Name ="Omaniku nimi")]
        public string OwnerName { get; set; } 
        
    
        [Required(ErrorMessage = "Kardi number on nõutav")]
        [Display(Name ="Kardi number")]
        [EstonianDebitCard]
        public string CardNumber{ get; set; } 
        
        
        
        [Required(ErrorMessage = "Kuu on nõutav")]
        public string ExpirationMonth { get; set; }
        
        
        [Required(ErrorMessage = "Asta on nõutav")]
        public string ExpirationYear { get; set; }

        

        [Required(ErrorMessage = "CVV kood on nõutav")]
        [Display(Name ="CVV kood")]
        [EstonianCVV]
        public string CvvCode{ get; set; } 
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