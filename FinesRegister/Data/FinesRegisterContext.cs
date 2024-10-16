using FinesRegister.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinesRegister.Data
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
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

    }
}
// INSERT INTO `AspNetUsers` (`Id`, `LastName`, `IsAdmin`, `CreatedAt`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`) VALUES
//     ('aece623c-ddd8-4233-8c90-6b516663b7fe', 'Dotskin', 1, '2024-10-11 21:21:25.958462', 'Maksim', 'MAKSIM', 'admin@add.com', 'ADMIN@ADD.COM', 0, 'AQAAAAIAAYagAAAAEOp+hSmEI+QMobr4kbUtTaqcxgRX30F5h4ELvVeCmezrJcYccNAZSVl2zK1eLe0jMg==', '2Y6V6CS2KWI7XEKMZINA34BN6NNMM4H3', 'd41d7b7e-7ce8-47a1-a602-e67c96515bff', '55553605', 0, 0, NULL, 1, 0),
// ('f9ace418-afdc-44ef-a12f-c9cb73a9e718', 'Dotskin', 0, '2024-10-11 22:18:09.843685', 'Maksim5', 'MAKSIM5', 'hui@add.com', 'HUI@ADD.COM', 0, 'AQAAAAIAAYagAAAAEJtw27PBA5W4miUjTB4mtq7wr9M4QlJuSXgMwNT6p32Q5M6Lp7MyN8cSp1wF2gfKug==', 'GQR2NU2MJNM5ERGOTYGJTPT3KRY7DV6B', '613fe1f0-a39c-4e40-8d23-0f956d3ca394', '55553605', 0, 0, NULL, 1, 0),
// ('not defined', 'not defined', 0, '2024-10-11 21:19:38.000000', 'not defined', 'not defined', 'not defined', 'not defined', 0, 'not defined', NULL, NULL, NULL, 0, 0, NULL, 0, 0);
// INSERT INTO `Cars` (`Id`, `Number`, `UserId`) VALUES
//     (2, '123-ADM', 'aece623c-ddd8-4233-8c90-6b516663b7fe'),
// (3, '143-BTD', 'aece623c-ddd8-4233-8c90-6b516663b7fe'),
// (4, '143-BTD', 'aece623c-ddd8-4233-8c90-6b516663b7fe'),
// (5, '123-ADL', 'aece623c-ddd8-4233-8c90-6b516663b7fe'),
// (6, '134-BTD', 'f9ace418-afdc-44ef-a12f-c9cb73a9e718');
// INSERT INTO `Fines` (`Id`, `IssueDate`, `DueDate`, `Amount`, `Reason`, `IsPaid`, `CarId`) VALUES
//     (2, '2024-10-11 22:33:00.000000', '2024-10-26 22:33:00.000000', 23, 'nene', 0, 2);