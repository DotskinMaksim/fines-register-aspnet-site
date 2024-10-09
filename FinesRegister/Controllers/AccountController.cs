using FinesRegister.Models;
using Microsoft.AspNetCore.Authorization; // Для использования атрибута AllowAnonymous
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Необходимо для использования EF Core
using System.Linq; // Необходимо для использования LINQ
using System.Threading.Tasks;
using FinesRegister.Models.Models;

namespace FinesRegister.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager; // Для управления пользователями
        private readonly SignInManager<IdentityUser> _signInManager; // Для аутентификации

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous] // Убедитесь, что это пространство имен импортировано
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(AccountViewModels.RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new FinesRegisterContext())
                {
                    // Проверка наличия владельца автомобиля с данным номером
                    var existingCarNumber = db.Cars
                        .FirstOrDefault(c => c.Number == model.CarNumber && c.UserId != 0); // Убедитесь, что UserId не равен 0

                    if (existingCarNumber != null)
                    {
                        // Добавляем ошибку модели, если такой владелец уже существует
                        ModelState.AddModelError("", "Владелец с данным номером автомобиля уже зарегистрирован.");
                        return View(model); // Возвращаем пользователя на страницу регистрации
                    }

                    // Проверка наличия владельца с таким же email
                    var existingEmail = db.Users
                        .FirstOrDefault(u => u.Email == model.Email); // Проверка существующего email
                    if (existingEmail != null)
                    {
                        // Добавляем ошибку модели, если такой владелец уже существует
                        ModelState.AddModelError("", "Владелец с данным эмейлом уже зарегистрирован.");
                        return View(model); // Возвращаем пользователя на страницу регистрации
                    }
                }

                // Создаем нового пользователя
                var user = new ApplicationUser()
                {
                    Email = model.Email,
                    UserName = model.FirstName, 
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                };
                
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
                
                
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Сохраняем владельца автомобиля в базе данных
                    using (var db = new FinesRegisterContext())
                    {
                        var car = await db.Cars.FirstOrDefaultAsync(c => c.Number == model.CarNumber); // Ищем автомобиль
                        if (car != null)
                        {
                            car.UserId = user.Id; // Устанавливаем ID пользователя
                            await db.SaveChangesAsync(); // Сохраняем изменения асинхронно
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }

                AddErrors(result); // Обработка ошибок
            }

            // Если произошла ошибка, снова отображаем форму регистрации
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
