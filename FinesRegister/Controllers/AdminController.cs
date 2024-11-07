using System.Runtime.InteropServices.JavaScript;
using System.Text;
using FinesRegister.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using FinesRegister.Data;
using FinesRegister.Attributes;
using FinesRegister.Services.Email;
using FinesRegister.Services.SMS;

namespace FinesRegister.Controllers;


[Authorize]
[AdminOnly]
public class AdminController : Controller
{
    private readonly FinesRegisterContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly EmailService _emailService;
    private readonly ISmsService _smsService;
    

    public AdminController(FinesRegisterContext dbContext, UserManager<User> userManager,
        EmailService emailService, ISmsService smsService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _emailService = emailService;
        _smsService = smsService;
        
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _dbContext.LoginLogs
            .Include(log => log.User)
            .OrderByDescending(log => log.LoginTime)
            .ToListAsync();

        return View(logs);  
    }

    public async Task<IActionResult> Fines() 
    {
        var fines = await _dbContext.Fines.Include(f => f.Car).ToListAsync();
        return View(fines);
    }
    public async Task<IActionResult> NotifyBySms(int id)
    {
        var _fine = await _dbContext.Fines
            .Include(f => f.Car)
            .ThenInclude(c => c.User) // Загружаем пользователя, связанного с автомобилем
            .FirstOrDefaultAsync(f => f.Id == id);

        if (_fine.Car.User.Id == "not defined")
        {
            TempData["AlertMessage"] = "Sõiduki kasutaja ei ole registreeritud";
            return RedirectToAction("Fines", "Admin");
        }
        if (_fine == null)
        {
            throw new Exception("Fine not found");
        }
        string phoneNumber = "+372"+_fine.Car.User.PhoneNumber;
        
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Services/SMS/SmsTemplates/FineNotification.txt");

        string message = await System.IO.File.ReadAllTextAsync(templatePath,Encoding.UTF8);
        
        message = _fine.FormatMessageBody(message);
        
        await _smsService.SendSmsAsync(phoneNumber, message);
        return RedirectToAction("Fines", "Admin");
    }

    public async Task<IActionResult> NotifyByEmail(int id)
    {
        
        var _fine = await _dbContext.Fines
            .Include(f => f.Car)
            .ThenInclude(c => c.User) // Загружаем пользователя, связанного с автомобилем
            .FirstOrDefaultAsync(f => f.Id == id);   
        
        if (_fine.Car.User.Id == "not defined")
        {
            TempData["AlertMessage"] = "Sõiduki kasutaja ei ole registreeritud";
            return RedirectToAction("Fines", "Admin");
        }
        if (_fine == null)
        {
            // Обработка случая, когда штраф не найден
            throw new Exception("Fine not found");
        }

        string email = _fine.Car?.User?.Email; // Используйте оператор безопасного доступа (?.)

        if (string.IsNullOrEmpty(email))
        {
            // Обработка случая, когда адрес электронной почты не найден
            throw new Exception("Email not found for the user associated with the fine");
        }

        await _emailService.SendConfirmationEmail(email, "FineNotification", "Trahvi teade", fine: _fine);


        
        return RedirectToAction("Fines", "Admin");
    }
    
    public async Task<IActionResult> NotifyFine(int id)
    {
        // Отправка уведомления по SMS
        await NotifyBySms(id);
    
        // Отправка уведомления по электронной почте
        await NotifyByEmail(id);
    
        // Перенаправление на страницу со списком штрафов
        return RedirectToAction("Fines", "Admin");
    }


    public async Task<IActionResult> Cars() //CARS
    {
        
        var cars = await _dbContext.Cars.Include(c => c.User).ToListAsync(); // Получаем список машин
        return View(cars);
        
           
        
    }
    
    // GET: /Admin/CarEdit/5
    public async Task<IActionResult> CarEdit(int id)
    {
        var car = await _dbContext.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        // Получаем список пользователей для выпадающего списка
        var users = _userManager.Users.Select(u => new SelectListItem
        {
            Value = u.Id,
            Text = u.UserName
        }).ToList();

        var editModel = new CarEditViewModel
        {
            Id = car.Id,
            Number = car.Number,
            UserId = car.UserId,
            Users = users  // Передаем список пользователей в модель
        };

        return View(editModel);
    }

// POST: /Admin/CarEdit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CarEdit(CarEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            var car = await _dbContext.Cars.FindAsync(model.Id);
            if (car == null)
            {
                return NotFound();
            }

            car.Number = model.Number;
            car.UserId = model.UserId;  // Сохраняем выбранного пользователя

            _dbContext.Update(car);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Cars", "Admin");
        }

        // В случае ошибки, снова передаем список пользователей
        model.Users = _userManager.Users.Select(u => new SelectListItem
        {
            Value = u.Id,
            Text = u.UserName
        }).ToList();

        return View(model);

    }


// GET: /Admin/CarDelete/5
    public async Task<IActionResult> CarDelete(int id) //CAR DELETE
    {
        var car = await _dbContext.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        var deleteModel = new CarDeleteViewModel
        {
            Id = car.Id,
            Number = car.Number,
            UserId = car.UserId
        };

        return View(deleteModel);
    }

    
// POST: /Admin/CarDeleteConfirmed/5
    [HttpPost, ActionName("CarDeleteConfirmed")] //CAR DELETE CONFIRMED
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CarDeleteConfirmed(int id)
    {
        var car = await _dbContext.Cars.FindAsync(id);
        if (car != null)
        {
            _dbContext.Cars.Remove(car);
            await _dbContext.SaveChangesAsync();
        }
        return RedirectToAction("Cars", "Admin");
    }
    
    // GET: /Admin/CarCreate
    public IActionResult CarCreate()
    {
        var users = _userManager.Users.Select(u => new SelectListItem
        {
            Value = u.Id,         // ID пользователя
            Text = u.UserName     // Имя пользователя или другое значение, которое вы хотите отображать
        }).ToList();

        var model = new CarCreateViewModel
        {
            Users = users
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CarCreate(CarCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var car = new Car
            {
                Number = model.Number,
                UserId = model.UserId // Сохраняем выбранного пользователя
            };

            _dbContext.Cars.Add(car);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Cars)); // Перенаправление на список автомобилей
        }

        // Если есть ошибка, снова заполняем пользователей
        model.Users = _userManager.Users.Select(u => new SelectListItem
        {
            Value = u.Id,
            Text = u.UserName
        }).ToList();

        return View(model);
    }


    
    
   

// GET: /Admin/FineDelete/5
    
    public async Task<IActionResult> FineDelete(int id) //FINE DELETE
    {
        var fine = await _dbContext.Fines.FindAsync(id);
        if (fine == null)
        {
            return NotFound();
        }

        var deleteModel = new FineDeleteViewModel
        {
            Id = fine.Id,
            IssueDate = fine.IssueDate,
            DueDate = fine.DueDate,
            Amount = fine.Amount,
            Reason = fine.Reason
        };

        return View(deleteModel);
    }

// POST: /Admin/FineDeleteConfirmed/5
    [HttpPost, ActionName("FineDeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FineDeleteConfirmed(int id) //FINE DELETE CONFIRMED
    {
        var fine = await _dbContext.Fines.FindAsync(id);
        if (fine != null)
        {
            _dbContext.Fines.Remove(fine);
            await _dbContext.SaveChangesAsync();
        }
        return RedirectToAction("Fines", "Admin");
    }
    
    
   // GET: /Admin/FineCreate
   public async Task<IActionResult> FineCreate()
    {
    // Получаем список автомобилей
    var cars = await _dbContext.Cars.Select(c => new SelectListItem
    {
        Value = c.Id.ToString(),
        Text = c.Number
    }).ToListAsync();

    var model = new FineCreateViewModel
    {
        Cars = cars,
    };

    return View(model);
    }

// POST: /Admin/FineCreate

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FineCreate(FineCreateViewModel model)
    {
        // Убедимся, что дата актуальна и корректна
        if (model.IssueDate == default) // Если дата не задана, устанавливаем её в сегодня
        {
            model.IssueDate = DateOnly.FromDateTime(DateTime.Today); // Преобразуем DateTime в DateOnly
        }


        if (model.CarNumber != null)
        {
            try
            {
                var car = await _dbContext.Cars
                    .FirstOrDefaultAsync(c => c.Number == model.CarNumber);
                if (car == null)
                {
                    ViewBag.ErrorMessage = "Auto ei leitud";
                    return View(model); // Возвращаем модель, чтобы сохранить введенные данные
                }
                model.CarId = car.Id;
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Auto ei leitud";
                return View(model); // Возвращаем модель, чтобы сохранить введенные данные
            }
        }

        var fine = new Fine
        {
            IssueDate = model.IssueDate,
            DueDate = model.IssueDate.AddDays(15),
            CarId = model.CarId
        };

        if (!string.IsNullOrEmpty(model.ViolationType)) // Проверка на пустое значение
        {
            switch (model.ViolationType)
            {
                case "SpeedExcess":
                    fine.Reason = $"Kiirusülempiir {model.Excess} km/h";
                    fine.Amount = model.CalculateFine();
                    break;
                case "WrongStay":
                    fine.Reason = "Peatus vales kohas";
                    fine.Amount = 100;
                    break;
            }
        }

        // Добавляем штраф в контекст и сохраняем изменения
        _dbContext.Fines.Add(fine);
        await _dbContext.SaveChangesAsync();

        // Перенаправление на список штрафов
        return RedirectToAction("NotifyFine", "Admin", new { id = fine.Id });


    }


    // GET: /Admin/FineEdit/5
    public async Task<IActionResult> FineEdit(int id) //FINE EDIT
    {
        var fine = await _dbContext.Fines.FindAsync(id);
        if (fine == null)
        {
            return NotFound();
        }

        var cars = _dbContext.Cars.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Number
        }).ToList();

        var editModel = new FineEditViewModel
        {
            Id = fine.Id,
            IssueDate = fine.IssueDate,
            DueDate = fine.DueDate,
            Amount = fine.Amount,
            Reason = fine.Reason,
            IsPaid = fine.IsPaid,
            CarId = fine.CarId,
            Cars = cars // Передаем список автомобилей в модель
        };

        return View(editModel);
    }

    // POST: /Admin/FineEdit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FineEdit(FineEditViewModel model) //FINE EDIT
    {
        if (ModelState.IsValid)
        {
            var fine = await _dbContext.Fines.FindAsync(model.Id);
            if (fine == null)
            {
                return NotFound();
            }

            fine.IssueDate = model.IssueDate;
            fine.DueDate = model.DueDate;
            fine.Amount = model.Amount;
            fine.Reason = model.Reason;
            fine.IsPaid = model.IsPaid;
            fine.CarId = model.CarId;

            _dbContext.Update(fine);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Fines", "Admin");
        }

        // В случае ошибки, снова получаем список автомобилей
        model.Cars = _dbContext.Cars.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Number
        }).ToList();

        return View(model);
    }
    
}