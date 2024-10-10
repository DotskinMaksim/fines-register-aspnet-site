using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using FinesRegister.Validators;

namespace FinesRegister.Models
{

    public class CarEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Number is required")]
        [StringLength(50, ErrorMessage = "Number can't be longer than 50 characters.")]
        public string Number { get; set; }

        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
    }
    
    public class CarDeleteViewModel
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string UserId { get; set; }
    }
    public class CarCreateViewModel
    {
        [Required(ErrorMessage = "Number is required")]
        [StringLength(50, ErrorMessage = "Number can't be longer than 50 characters.")]
        [EstonianCarNumber]
        public string Number { get; set; }
        
        
    }
    public class FineEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Issue Date is required")]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "Due Date is required")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public float Amount { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(250, ErrorMessage = "Reason can't be longer than 250 characters.")]
        public string Reason { get; set; }

        public bool IsPaid { get; set; }
    }

    public class FineDeleteViewModel
    {
        public int Id { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public float Amount { get; set; }
        public string Reason { get; set; }
    }
    public class FineCreateViewModel
    {
        [Required(ErrorMessage = "Issue date is required")]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public float Amount { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        public string Reason { get; set; }

        public bool IsPaid { get; set; } = false;

        [Required(ErrorMessage = "Car ID is required")]
        public int CarId { get; set; }
    }
}
