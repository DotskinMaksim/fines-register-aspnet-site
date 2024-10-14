using System.Runtime.InteropServices.JavaScript;
using FinesRegister.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using FinesRegister.Data;


namespace FinesRegister.Controllers;


[Authorize]
[AdminOnly]
public class AdminController : Controller
{
    private readonly FinesRegisterContext _dbContext;
    private readonly UserManager<User> _userManager;


    public AdminController(FinesRegisterContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;

    }
    
    
    
    public async Task<IActionResult> Fines() 
    {
        var fines = await _dbContext.Fines.Include(f => f.Car).ToListAsync();
        return View(fines);
    }

    
    public async Task<IActionResult> Cars() //CARS
    {
        
        var cars = await _dbContext.Cars.ToListAsync(); // Получаем список машин
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
    public IActionResult FineCreate() //FINE CREATE
    {
        var cars = _dbContext.Cars.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Number
        }).ToList();

        var model = new FineCreateViewModel
        {
            Cars = cars
        };

        return View(model);
    }

    // POST: /Admin/FineCreate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FineCreate(FineCreateViewModel model) //FINE CREATE
    {
        if (ModelState.IsValid)
        {
            var fine = new Fine
            {
                IssueDate = model.IssueDate,
                DueDate = model.IssueDate.AddDays(15),
                Amount = model.Amount,
                Reason = model.Reason,
                CarId = model.CarId
            };

            _dbContext.Fines.Add(fine);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Fines)); // Перенаправление на список штрафов
        }

        // В случае ошибки, снова получаем список автомобилей
        model.Cars = _dbContext.Cars.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Number
        }).ToList();

        return View(model);
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