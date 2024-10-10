using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Runtime.CompilerServices;

namespace FinesRegister.Models;

public class Fine
{
    public int Id { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public float Amount { get; set; }
    public string Reason { get; set; }
    public bool IsPaid { get; set; } = false;
    public int CarId { get; set; }
    
}