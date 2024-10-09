using FinesRegister.Validators;
using Microsoft.EntityFrameworkCore;

namespace FinesRegister.Models;

public class CarNumber
{
    public int Id { get; set; }
    
    [EstonianCarNumber]
    public string Number { get; set; }
    
    public int UserId { get; set; }
}