using System.ComponentModel.DataAnnotations.Schema;
using FinesRegister.Attributes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace FinesRegister.Models;

public class Car
{
    public int Id { get; set; }
    
    
    [Display(Name ="Number")]
    public string Number { get; set; }
    
    
    [Display(Name ="Kasutaja ID")]
    [ForeignKey("User")]
    public string UserId { get; set; }
    
    
    public User User { get; set; }
    
    
    public ICollection<Fine> Fines { get; set; } 

}