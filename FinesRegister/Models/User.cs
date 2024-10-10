using System.ComponentModel.DataAnnotations;
using FinesRegister.Validators;
using System.Security.Cryptography;
using System.Text;


using Microsoft.AspNetCore.Identity;


namespace FinesRegister.Models;

public class User : IdentityUser
{
    public int Id { get; set; }
    public string LastName { get; set; }
    public bool IsAdmin { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    
}