using System.Security.Claims;
using FinesRegister.Models;
using FinesRegister.Data;
using FinesRegister.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;


namespace FinesRegister.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly FinesRegisterContext _dbContext;
        private readonly ILogger<AccountController> _logger;
        private readonly EmailService _emailService;



        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            FinesRegisterContext dbContext, ILogger<AccountController> logger,
            EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _logger = logger;
            _emailService = emailService;


        }

        [Authorize]
        public async Task<IActionResult> Fines()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Получаем ID текущего пользователя

            // Фильтруем штрафы по пользователю
            var fines = await _dbContext.Fines
                .Include(f => f.Car)
                .Where(f => f.Car.UserId == userId)
                .ToListAsync();

            return View(fines); // Возвращаем только штрафы, относящиеся к текущему пользователю
        }
        
        
       
        
        [Authorize]
        [HttpPost]
        public IActionResult PayFines(string fineIds)
        {
            // Десериализация идентификаторов штрафов из JSON
            var fineIdsList = JsonConvert.DeserializeObject<List<int>>(fineIds);

            // Получаем все штрафы с указанными идентификаторами
            var fines = _dbContext.Fines
                .Where(f => fineIdsList.Contains(f.Id) && !f.IsPaid)
                .ToList();

            var totalAmount = fines.Sum(f => f.Amount);

            var model = new PaymentViewModel
            {
                Fines = fines,
                TotalAmount = totalAmount,
                PaymentMethods = _dbContext.PaymentMethods
                    .Where(p => p.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .ToList() // Получение списка методов оплаты
            };

            return View("PayFines", model);
        }
        
        // Завершение платежа и отметка штрафов как оплаченных
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CompletePayment(string fineIds)
        {
            
            var fineIdsList = JsonConvert.DeserializeObject<List<int>>(fineIds);

            var fines = _dbContext.Fines
                .Where(f => fineIdsList.Contains(f.Id) && !f.IsPaid)
                .ToList();
            
            fines.ForEach(f => f.IsPaid = true);
            foreach (var fine in fines)
            {
                _dbContext.Update(fine);
                await _dbContext.SaveChangesAsync();
            }
            await Task.Delay(3000); 
            return RedirectToAction("PaymentSuccessful");
        }

        public IActionResult PaymentSuccessful()
        {
            return View();
        }
       

        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult AddPaymentMethod()
        {
            return View();
        }
        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPaymentMethod(AddPaymentMethodViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var paymentMethod = new PaymentMethod
                {
                    OwnerName = model.OwnerName,
                    CardNumber = model.CardNumber,
                    ExpirationDate = $"{model.ExpirationMonth}/{model.ExpirationYear}",
                    CvvCode = model.CvvCode.ToString(),
                    UserId = userId
                };

                await _dbContext.PaymentMethods.AddAsync(paymentMethod);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Fines");
            }

            return View("AddPaymentMethod", model);
        }


        public IActionResult Logout()
        {
            _signInManager.SignOutAsync(); // Выход пользователя
            return RedirectToAction("Index", "Home"); // Перенаправление на главную страницу после выхода
        }
        
        // Метод для удаления всех пользователей с неподтвержденной почтой
        private async Task DeleteInactiveUsersAsync()
        {
            var inactiveUsers = await _dbContext.Users
                .Where(u => !u.EmailConfirmed)
                .ToListAsync(); // Найдите всех пользователей, которые не подтвердили почту и зарегистрировались более 24 часов назад

            if (inactiveUsers.Any())
            {
                _dbContext.Users.RemoveRange(inactiveUsers); // Удаляем пользователей из БД
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Eemaldatud {inactiveUsers.Count} passiivset kasutajat");
            }
        }

        public async Task<IActionResult> SendConfirmationCode()
        {
            var email = TempData["Email"]?.ToString();
            
            // Генерация и отправка кода подтверждения
            string confirmCode = _emailService.GenerateRandomNumbers(6);
            await _emailService.SendConfirmationEmail(email,  "ConfirmationCode",
                    "Kinnitage registreerimine",confirmCode);

            // Сохраняем код в TempData для проверки в подтверждении
            TempData["ConfirmCode"] = confirmCode;

            return RedirectToAction("ConfirmRegister");
        }


        

        // Метод для страницы входа
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await DeleteInactiveUsersAsync(); // Удаляем неактивированных пользователей

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
                    _logger.LogWarning("E-postiga kasutajat ei leitud: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Vale e-posti aadress või parool.");
                    return View(model);
                } 
                if (user.EmailConfirmed == false)
                {
                    _logger.LogWarning("Kasutaja pole oma kontot kinnitanud: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Konto pole kinnitatud.");
                    return View(model);
                }

                // Проверка блокировки пользователя
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("Kasutaja {Email} on lukustatud.", model.Email);
                    ModelState.AddModelError(string.Empty, "Kasutaja on blokeeritud.");
                    return View(model);
                }

                // Проверка входа
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe,
                    lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var loginLog = new LoginLog
                    {
                        UserId = user.Id,
                        LoginTime = DateTime.UtcNow
                    };
                    _dbContext.LoginLogs.Add(loginLog);
                    _dbContext.SaveChanges();
                    _logger.LogInformation("Kasutaja {Email} logis edukalt sisse", model.Email);
                    return RedirectToAction("Index", "Home");
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(
                        "Kasutaja {Email} lukustatakse pärast liiga palju ebaõnnestunud sisselogimiskatseid",
                        model.Email);
                    ModelState.AddModelError(string.Empty,
                        "Kasutaja lukustati pärast liiga palju ebaõnnestunud sisselogimiskatseid");
                    return View(model);
                }
               
                    _logger.LogWarning("Kehtetu parool kasutajale: {Email}", model.Email);
                    ModelState.AddModelError(string.Empty, "Vale e-posti aadress või parool");
                    return View(model);
                
            }

            return View(model);
        }



        // GET: /Account/Register
        [HttpGet]
        // Метод для страницы регистрации
        public async Task<IActionResult> Register()
        {
            await DeleteInactiveUsersAsync(); // Удаляем неактивированных пользователей

            return View();
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Проверка наличия пользователя с таким же email
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", 
                        "See meil on juba registreeritud");
                    return View(model);
                }
                
                var car = await _dbContext.Cars
                    .FirstOrDefaultAsync(c => c.Number == model.CarNumber && c.UserId == "not defined");
                if (car == null)
                {
                    ModelState.AddModelError("",
                        "Selle numbrimärgiga sõidukit ei leitud või see on juba registreeritud");
                    return View(model);
                }

                // Создаем нового пользователя
                var user = new User
                {
                    Email = model.Email,
                    UserName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = false // Пользователь не подтвержден
                };

                // Создаем пользователя без активации
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    
                    // string confirmCode = _emailService.GenerateRandomNumbers(6);
                    // await _emailService.SendConfirmationEmail(model.Email, confirmCode, "confirmationCode", "Kinnitage registreerimine");
                    //
                    // // Сохраняем код в TempData для проверки в подтверждении
                    // TempData["UserId"] = user.Id;
                    //
                    // TempData["ConfirmCode"] = confirmCode;
                    // TempData["Email"] = model.Email;
                    //
                    // return RedirectToAction("ConfirmRegister");
                    TempData["UserId"] = user.Id;
                    TempData["Email"] = user.Email;
                    
                    return await SendConfirmationCode();
                }

                AddErrors(result);
            }

            return View(model);
        }

        // GET: /Account/ConfirmRegister
        [HttpGet]
        public IActionResult ConfirmRegister()
        {
            return View();
        }

        // POST: /Account/ConfirmRegister
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRegister(ConfirmCodeViewModel model)
        {
            // Получаем код из TempData
            var userId = TempData["UserId"]?.ToString();

            var confirmCode = TempData["ConfirmCode"]?.ToString();
           
            if (confirmCode == model.Code && !string.IsNullOrEmpty(userId))
            {
                // Находим пользователя по ID
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    // Подтверждаем email
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);

                    // Автовход после подтверждения
                    await _signInManager.SignInAsync(user, isPersistent: true);

                    return RedirectToAction("Index", "Home");
                }
            }

            // Если код неверный, возвращаем сообщение об ошибке
            ViewBag.ErrorMessage = "Kehtetu kinnituskood";
            return View();
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
