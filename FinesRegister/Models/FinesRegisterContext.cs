using Microsoft.EntityFrameworkCore;

namespace FinesRegister.Models;

public class FinesRegisterContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Fine> Fines { get; set; }
    public DbSet<Car> Cars { get; set; }

}