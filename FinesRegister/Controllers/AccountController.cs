using Microsoft.AspNetCore.Mvc;

namespace FinesRegister.Controllers;

public class AccountController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }
}