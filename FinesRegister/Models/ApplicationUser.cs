using Microsoft.AspNetCore.Identity;


namespace FinesRegister.Models;

public class ApplicationUser : IdentityUser
{
    public string LastName { get; set; }
    public bool IsAdmin { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
}

