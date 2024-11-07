using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FinesRegister.Models;

namespace FinesRegister.Attributes
{

    public class AdminOnlyAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();

            var currentUser = await userManager.GetUserAsync(context.HttpContext.User);

            if (currentUser == null || !currentUser.IsAdmin)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }
    }
}