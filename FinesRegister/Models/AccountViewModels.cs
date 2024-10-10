using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FinesRegister.Validators;

namespace FinesRegister.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required] [Display(Name = "Eesnimi")] public string FirstName { get; set; }

        [Required]
        [Display(Name = "Perenimi")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Parool")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Kinnita parool")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
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



        [Display(Name = "Remember me?")] public bool RememberMe { get; set; }

    }
}