using System.ComponentModel.DataAnnotations.Schema;
using FinesRegister.Validators;
using Microsoft.EntityFrameworkCore;

namespace FinesRegister.Models;

public class Car
{
    public int Id { get; set; }
    public string Number { get; set; }
    
    public string UserId { get; set; }
}