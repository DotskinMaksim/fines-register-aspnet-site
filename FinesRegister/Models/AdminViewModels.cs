using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using FinesRegister.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinesRegister.Models
{

    public class CarEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Number on nõutav")]
        [EstonianCarNumber]
        [Display(Name = "Number")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Kasutaja ID on nõutav")]
        [Display(Name = "Kasutaja ID")]
        public string UserId { get; set; }
        
        public IEnumerable<SelectListItem> Users { get; set; }

    }
    
    public class CarDeleteViewModel
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string UserId { get; set; }
    }
    public class CarCreateViewModel
    {
        [Required(ErrorMessage = "Number on nõutav")]
        [EstonianCarNumber]
        [Display(Name = "Number")]
        public string Number { get; set; }
        
        
        [Display(Name = "Kasutaja")]
        [Required(ErrorMessage = "Vali kasutaja.")]
        public string UserId { get; set; }
        
        public IEnumerable<SelectListItem> Users { get; set; }

        
    }
    public class FineEditViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Väljastamise kuupäev")]
        [Required(ErrorMessage = "Väljastamise kuupäev on nõutav")]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "Tähtaeg on nõutav")]
        [Display(Name = "Tähtaeg")]
        public DateTime DueDate { get; set; }

        
        [Required(ErrorMessage = "Summa on nõutav")]
        [Display(Name = "Summa")]
        public float Amount { get; set; }

        [Required(ErrorMessage = "Põhjus on nõutav")]
        [StringLength(250, ErrorMessage = "Põhjus ei tohi olla pikem kui 250 tähemärki")]
        [Display(Name = "Põhjus")]
        public string Reason { get; set; }

        
        [Display(Name ="Makse olek")]
        public bool IsPaid { get; set; }
        
        
        [Display(Name ="Auto ID")]
        public int CarId { get; set; }

        
        public IEnumerable<SelectListItem> Cars { get; set; } // Добавьте это свойство

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
        [Required(ErrorMessage = "Väljastamise kuupäev on nõutav")]
        [Display(Name = "Väljastamise kuupäev")]
        public DateTime IssueDate { get; set; }

        [Required(ErrorMessage = "Summa on nõutav")]
        [Display(Name = "Summa")]
        public float Amount { get; set; }

        [Required(ErrorMessage = "Põhjus on nõutav")]
        [Display(Name = "Põhjus")]
        public string Reason { get; set; }
        
        
        public bool IsPaid { get; set; } = false;

        [Required(ErrorMessage = "Auto ID on nõutav")]
        [Display(Name = "Auto ID")]
        public int CarId { get; set; }
        
        public IEnumerable<SelectListItem> Cars { get; set; } // Добавьте это свойство
    }
}
