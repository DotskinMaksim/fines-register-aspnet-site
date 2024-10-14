using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using FinesRegister.Data; // Подключите пространство имен вашего контекста базы данных
using FinesRegister.Models;
using Microsoft.AspNetCore.Identity;

public class PaymentMethodRequiredAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Получаем доступ к IServiceProvider из контекста
        var serviceProvider = context.HttpContext.RequestServices;

        // Получаем UserManager и контекст базы данных
        var userManager = serviceProvider.GetService<UserManager<User>>();
        var dbContext = serviceProvider.GetService<FinesRegisterContext>();

        // Получаем текущего пользователя
        var user = userManager.GetUserAsync(context.HttpContext.User).Result;

        if (user != null)
        {
            // Проверяем наличие методов оплаты у пользователя
            var paymentMethods = dbContext.PaymentMethods
                .Where(pm => pm.UserId == user.Id)
                .ToList();

            if (!paymentMethods.Any())
            {
                // Перенаправляем на страницу добавления метода оплаты
                context.Result = new RedirectToActionResult("PaymentMethodAdd", "Account", null);
            }
        }

        // Вызываем базовый метод
        base.OnActionExecuting(context);
    }
}