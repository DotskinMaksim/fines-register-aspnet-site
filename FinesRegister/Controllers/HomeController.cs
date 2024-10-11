using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FinesRegister.Models;
using Microsoft.AspNetCore.Identity;

namespace FinesRegister.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;


    public HomeController(ILogger<HomeController> logger, UserManager<User> userManager)
    {
        _logger = logger;
        _userManager = userManager;

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


    
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}