using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Runtime.CompilerServices;

namespace FinesRegister.Models;

public class Fine
{
    public int Id { get; set; }
    
    
    [Display(Name = "Väljastamise kuupäev")]
    public DateTime IssueDate { get; set; }
    
    
    
    [Display(Name = "Tähtaeg")]
    public DateTime DueDate { get; set; }
    
    
    [Display(Name = "Summa")]
    public float Amount { get; set; }
    
    
    [Display(Name ="Põhjus")]
    public string Reason { get; set; }
    
    
    [Display(Name ="On tasutud?")]
    public bool IsPaid { get; set; } = false;
    
    
    [Display(Name ="Auto ID")]
    public int CarId { get; set; }
    
}