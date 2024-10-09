using FinesRegister.Validators;
using Microsoft.EntityFrameworkCore;

namespace FinesRegister.Models;

public class Car
{
    public int Id { get; set; }
    
    public string Number { get; set; }
    
    public int UserId { get; set; }
}