using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinesRegister.Models;

public class PaymentMethod
{
    public int Id { get; set; }
    
    
    [Display(Name ="Omaniku nimi")]
    public string OwnerName { get; set; }  
    
    
    [Display(Name ="Kardi number")]
    public string CardNumber{ get; set; } 
    
    
    [Display(Name ="Kehtiv kuni")]
    public string ExpirationDate { get; set; }

    
    [Display(Name ="CVV kood")]
    public string CvvCode{ get; set; } 
    
    
    // [Display(Name ="Konto või kaardi number")]
    // public string AccountNumber { get; set; }  = null;
    
    

    [ForeignKey("User")]
    public string UserId { get; set; }  // Связь с пользователем
    public User User { get; set; }  // Связь с моделью пользователя (предполагается, что у вас есть модель пользователя)
}
