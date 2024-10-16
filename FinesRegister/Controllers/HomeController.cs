using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FinesRegister.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinesRegister.Data;
using FinesRegister.Attributes;



namespace FinesRegister.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly FinesRegisterContext _dbContext;


    public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, FinesRegisterContext dbContext)
    {
        _logger = logger;
        _userManager = userManager;
        _dbContext = dbContext;

    }

    // public async Task<IActionResult> Index()
    // {
    //     var currentUser = await _userManager.GetUserAsync(User);
    //
    //     // Если пользователь найден, то проверяем IsAdmin, иначе по умолчанию false
    //     ViewBag.IsAdmin = currentUser != null && currentUser.IsAdmin == true; // Учтите, что IsAdmin == 1
    //
    //     return View();
    // }
    public IActionResult Index()
    {
        return View();
    }
    
    
    [Authorize]
    [PaymentMethodRequired]
    public IActionResult FinePay()
    {
        
        return View();
    }


    
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    [HttpPost]
    public async Task<IActionResult> Index(string carNumber)
    {
        if (string.IsNullOrEmpty(carNumber))
        {
            return View(Enumerable.Empty<Fine>());
        }

        var fines = await _dbContext.Fines
            .Where(f => f.Car.Number == carNumber)
            .Include(f => f.Car)
            .ToListAsync(); 
      
        return View(fines);
    }
    

}