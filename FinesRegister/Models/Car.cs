using System.ComponentModel.DataAnnotations.Schema;
using FinesRegister.Validators;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinesRegister.Models;

public class Car
{
    public int Id { get; set; }
    
    
    [Display(Name ="Number")]
    public string Number { get; set; }
    
    [Display(Name ="Kasutaja ID")]
    public string UserId { get; set; }
}