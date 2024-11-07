using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore.Mvc;
using FinesRegister.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinesRegister.Data;
using FinesRegister.Attributes;
using Newtonsoft.Json;

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
    
    public async Task<IActionResult> Index()
    {
        
        if (User.Identity.IsAuthenticated)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.IsAdmin)
            {
                return RedirectToAction("Index", "Admin");
            }

            var userId = _userManager.GetUserId(User);
            var userFines = await _dbContext.Fines
                    .Include(f => f.Car)
                    .Where(f => f.Car.UserId == userId)
                    .ToListAsync();
            
            ViewBag.UserId = userId;

            ViewBag.UserFines = userFines;
            
        }

        return View();
    }
    public IActionResult FAQ()
    {
        var questions = _dbContext.Questions.ToList();
        return View(questions);
    }
  
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    [HttpPost]
    public async Task<IActionResult> Index(string carNumber)
    {
        var fines = await _dbContext.Fines
            .Include(f => f.Car)
            .Where(f => f.Car.Number == carNumber)
            .ToListAsync();
        return View(fines);
    }
}