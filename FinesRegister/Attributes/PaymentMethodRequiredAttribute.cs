using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using FinesRegister.Data; 
using FinesRegister.Models;
using Microsoft.AspNetCore.Identity;

namespace FinesRegister.Attributes
{

    public class PaymentMethodRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var serviceProvider = context.HttpContext.RequestServices;

            var userManager = serviceProvider.GetService<UserManager<User>>();
            var dbContext = serviceProvider.GetService<FinesRegisterContext>();

            var user = userManager.GetUserAsync(context.HttpContext.User).Result;

            if (user != null)
            {
                var paymentMethods = dbContext.PaymentMethods
                    .Where(pm => pm.UserId == user.Id)
                    .ToList();

                if (!paymentMethods.Any())
                {
                    context.Result = new RedirectToActionResult("PaymentMethodAdd", "Account", null);
                }
            }

            base.OnActionExecuting(context);
        }
    }
}