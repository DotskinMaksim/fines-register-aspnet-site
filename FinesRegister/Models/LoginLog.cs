namespace FinesRegister.Models;

public class LoginLog
{
    public int Id { get; set; }
    public string UserId { get; set; } 
    public DateTime LoginTime { get; set; } 
    
    public virtual User User { get; set; }

}