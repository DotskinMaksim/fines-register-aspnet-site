using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Runtime.CompilerServices;

namespace FinesRegister.Models;

public class Fine
{
    public int Id { get; set; }
    
    [DataType(DataType.Date)] 
    public DateTime IssueDate { get; set; }
    
    [DataType(DataType.Date)] 
    public DateTime DueDate { get; set; }
    public float Amount { get; set; }
    public string Reason { get; set; }
    public bool IsPaid { get; set; } = false;
    public int CarNumberId { get; set; }
    
    
    public Fine()
    {
        DueDate = IssueDate.AddDays(15);
    }
}