using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinesRegister.Models
{
    public class FinesRegisterContext : IdentityDbContext<User, IdentityRole, string>
    {
        public FinesRegisterContext(DbContextOptions<FinesRegisterContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<Car> Cars { get; set; }
    }
}