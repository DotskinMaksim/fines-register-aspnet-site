using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinesRegister.Models;

public class Fine
{
    public int Id { get; set; }
    
    
    [Display(Name = "Saanud")]
    public DateTime IssueDate { get; set; }
    
    
    
    [Display(Name = "Tähtaeg")]
    public DateTime DueDate { get; set; }
    
    
    [Display(Name = "Summa")]
    public float Amount { get; set; }
    
    
    [Display(Name ="Põhjus")]
    public string Reason { get; set; }
    
    
    [Display(Name ="Makse olek")]
    public bool IsPaid { get; set; } = false;
    
    
    [Display(Name ="Auto ID")]
    [ForeignKey("Car")]
    public int CarId { get; set; }
    
    public Car Car { get; set; }  // Связь с автомобилем

    
}