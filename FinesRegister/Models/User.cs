using System.ComponentModel.DataAnnotations;
using FinesRegister.Validators;

namespace FinesRegister.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    
    [EstonianPhoneNumber]
    [Required]
    public string PhoneNumber { get; set; }
    
    public string PasswordHash { get; set; }
    public bool IsAdmin { get; set; } = false;
    public int CarNumberId { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}