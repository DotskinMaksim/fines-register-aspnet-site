using System.Net.Mime;

namespace FinesRegister.Models;

public class Fine
{
    public int Id { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public int Amount { get; set; }
    public string Reason { get; set; }
    public bool IsPaid { get; set; } = false;
    public int CarNumberId { get; set; }
    
    
    public Fine()
    {
        DueDate = IssueDate.AddDays(15);
    }
}