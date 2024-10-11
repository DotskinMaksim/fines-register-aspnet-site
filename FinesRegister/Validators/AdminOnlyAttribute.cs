using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FinesRegister.Models;

public class AdminOnlyAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Получаем сервис UserManager через DI
        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();

        // Получаем текущего пользователя
        var currentUser = await userManager.GetUserAsync(context.HttpContext.User);

        if (currentUser == null || !currentUser.IsAdmin)
        {
            // Если пользователь не администратор, перенаправляем на страницу "Доступ запрещен"
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
        }
    }
}