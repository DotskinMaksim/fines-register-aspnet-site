using System.ComponentModel.DataAnnotations;
using FinesRegister.Validators;
using System.Security.Cryptography;
using System.Text;


using Microsoft.AspNetCore.Identity;

//FineRegister fnuf hkki arjx lqwm
namespace FinesRegister.Models;

public class User : IdentityUser
{
    
    [Display(Name = "Kasutaja")]
    public override string UserName { get; set; }
    public string LastName { get; set; }
    public bool IsAdmin { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public ICollection<Car> Cars { get; set; } 
    
    public ICollection<PaymentMethod> PaymentMethods { get; set; }  // Связь с методами оплаты
    
    
    
    
}
