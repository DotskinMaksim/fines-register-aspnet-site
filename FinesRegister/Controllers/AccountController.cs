using FinesRegister.Models;
using Microsoft.AspNetCore.Authorization; // Для использования атрибута AllowAnonymous
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Необходимо для использования EF Core


namespace FinesRegister.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager; // Для управления пользователями
        private readonly SignInManager<User> _signInManager; // Для аутентификации
        private readonly FinesRegisterContext _dbContext;
        // private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AccountController> _logger; // Добавляем логгер



        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, FinesRegisterContext dbContext, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            // _passwordHasher = passwordHasher;
            _logger = logger; // Инициализируем логгер


        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        // // GET
        // public IActionResult Index()
        // {
        //     return View();
        // }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // Выход пользователя
            return RedirectToAction("Index", "Home"); // Перенаправление на главную страницу после выхода
        }

        
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl; // Сохраняем returnUrl в ViewBag для использования в представлении
            return View();
        }
        
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
        // Попробуйте найти пользователя по email
                var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user == null)
                {
                    _logger.LogWarning("No user found with email: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Неправильный адрес электронной почты или пароль.");
                    return View(model);
                }

                // Проверка блокировки пользователя
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("User {Email} is locked out.", model.Email);
                    ModelState.AddModelError(string.Empty, "Пользователь заблокирован.");
                    return View(model);
                }

                // Проверка входа
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} logged in successfully.", model.Email);
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {Email} is locked out after too many failed login attempts.", model.Email);
                    ModelState.AddModelError(string.Empty, "Пользователь заблокирован после слишком большого количества неудачных попыток входа.");
                    return View(model);
                }
                else
                {
                    _logger.LogWarning("Invalid password for user: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Неправильный адрес электронной почты или пароль.");
                    return View(model);
                }
            }

            return View(model);
        }



        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Проверка наличия владельца с таким же email
                var existingEmail = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email); // Проверка существующего email
                if (existingEmail != null)
                {
                    // Добавляем ошибку модели, если такой владелец уже существует
                    ModelState.AddModelError("", "Владелец с данным эмейлом уже зарегистрирован.");
                    return View(model); // Возвращаем пользователя на страницу регистрации
                }

                // Создаем нового пользователя
                var user = new User()
                {
                    Email = model.Email,
                    UserName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    PasswordHash = model.Password
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);

                    // Проверка наличия автомобиля с данным номером, у которого нет владельца
                    var car = await _dbContext.Cars
                        .FirstOrDefaultAsync(c => c.Number == model.CarNumber && c.UserId=="not defined");

                    if (car != null)
                    {
                        // Присваиваем новому пользователю автомобиль
                        car.UserId = user.Id;
                        await _dbContext.SaveChangesAsync(); // Сохраняем изменения
                    }
                    else
                    {
                        // Если автомобиля с таким номером нет или у него уже есть владелец
                        ModelState.AddModelError("", "Автомобиль с данным номером не найден или уже зарегистрирован.");
                        return View(model);
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
