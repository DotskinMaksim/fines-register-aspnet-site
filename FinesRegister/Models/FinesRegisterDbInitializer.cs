using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FinesRegister.Models
{
    public static class FinesRegisterDbInitializer
    {
        public static void Initialize(FinesRegisterContext context)
        {
           
            // Проверяем, существуют ли данные в базе
            if (context.Fines.Any())
            {
                return; // База данных уже инициализирована
            }

            // Здесь добавляем начальные данные
            var fines = new Fine[]
            {
                new Fine { IssueDate = DateTime.Now, DueDate = DateTime.Now.AddDays(15), Amount = 100, Reason = "Speeding", IsPaid = false, CarId = 1 },
                new Fine { IssueDate = DateTime.Now, DueDate = DateTime.Now.AddDays(15), Amount = 200, Reason = "Parking", IsPaid = false, CarId = 2 }
            };

            context.Fines.AddRange(fines);
            context.SaveChanges();
        }
    }
}