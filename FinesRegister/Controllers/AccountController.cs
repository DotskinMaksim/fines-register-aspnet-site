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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            var fines = await _dbContext.Fines
                .Include(f => f.Car)
                .Where(f => f.Car.UserId == userId)
                .ToListAsync();

            return View(fines); 
        }
        
        
       
        
        [Authorize]
        [HttpPost]
        public IActionResult PayFines(string fineIds)
        {
            var fineIdsList = JsonConvert.DeserializeObject<List<int>>(fineIds);

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
                    .ToList() 
            };

            return View("PayFines", model);
        }
        
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
            _signInManager.SignOutAsync(); 
            return RedirectToAction("Index", "Home"); 
        }
        
        private async Task DeleteInactiveUsersAsync()
        {
            var inactiveUsers = await _dbContext.Users
                .Where(u => !u.EmailConfirmed)
                .ToListAsync();

            if (inactiveUsers.Any())
            {
                var inactiveUserIds = inactiveUsers.Select(u => u.Id).ToList();
                var carsToUpdate = await _dbContext.Cars
                    .Where(c => inactiveUserIds.Contains(c.UserId))
                    .ToListAsync();

                foreach (var car in carsToUpdate)
                {
                    car.UserId = "not defined";
                }

                _dbContext.Cars.UpdateRange(carsToUpdate);
                await _dbContext.SaveChangesAsync();

                _dbContext.Users.RemoveRange(inactiveUsers);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Eemaldatud {inactiveUsers.Count} passiivset kasutajat");
            }
        }

        public async Task<IActionResult> SendConfirmationCode()
        {
            var email = TempData["Email"]?.ToString();
            
            string confirmCode = _emailService.GenerateRandomNumbers(6);
            await _emailService.SendConfirmationEmail(email,  "ConfirmationCode",
                    "Kinnitage registreerimine",confirmCode);

            TempData["ConfirmCode"] = confirmCode;

            return RedirectToAction("ConfirmRegister");
        }


        

        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await DeleteInactiveUsersAsync(); 

            ViewBag.ReturnUrl = returnUrl; 
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
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

                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("Kasutaja {Email} on lukustatud.", model.Email);
                    ModelState.AddModelError(string.Empty, "Kasutaja on blokeeritud.");
                    return View(model);
                }

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
        public async Task<IActionResult> Register()
        {
            await DeleteInactiveUsersAsync(); 

            return View();
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
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

                var user = new User
                {
                    Email = model.Email,
                    UserName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = false 
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    car.UserId = user.Id;
                    _dbContext.Update(car); 
                    await _dbContext.SaveChangesAsync();
                    
                   
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
            var userId = TempData["UserId"]?.ToString();
            var confirmCode = TempData["ConfirmCode"]?.ToString();

            if (confirmCode == model.Code && !string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);

                    await _signInManager.SignInAsync(user, isPersistent: true);

                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["ConfirmCode"] = confirmCode;
            TempData["UserId"] = userId;

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
