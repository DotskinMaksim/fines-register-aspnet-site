using FinesRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinesRegister.Controllers;

public class AdminController : Controller
{
    private readonly FinesRegisterContext _dbContext;

    public AdminController(FinesRegisterContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IActionResult> Fines() //FINES
    {
        var fines = await _dbContext.Fines.ToListAsync();
        return View(fines);
    }
    public async Task<IActionResult> Cars() //CARS
    {
        var cars = await _dbContext.Cars.ToListAsync();
        return View(cars);
    }
    
    
    // GET: /Admin/CarEdit/5
    public async Task<IActionResult> CarEdit(int id) //CAR EDIT
    {
        var car = await _dbContext.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        var editModel = new CarEditViewModel
        {
            Id = car.Id,
            Number = car.Number,
            UserId = car.UserId
        };

        return View(editModel);
    }

// POST: /Admin/CarEdit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CarEdit(CarEditViewModel model) //CAR EDIT
    {
        if (ModelState.IsValid)
        {
            var car = await _dbContext.Cars.FindAsync(model.Id);
            if (car == null)
            {
                return NotFound();
            }

            car.Number = model.Number;
            car.UserId = model.UserId;

            _dbContext.Update(car);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Cars", "Admin");
        }

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
    public IActionResult CarCreate() //CAR CREATE
    {
        return View();
    }

    // POST: /Admin/CarCreate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CarCreate(CarCreateViewModel model) //CAR CREATE
    {
        if (ModelState.IsValid)
        {
            
            
            var car = new Car
            {
                Number = model.Number,
                UserId = "not defined",
                
                
            };

            _dbContext.Cars.Add(car);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Cars)); // Перенаправление на список автомобилей
        }

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

        var editModel = new FineEditViewModel
        {
            Id = fine.Id,
            IssueDate = fine.IssueDate,
            DueDate = fine.DueDate,
            Amount = fine.Amount,
            Reason = fine.Reason,
            IsPaid = fine.IsPaid
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

            _dbContext.Update(fine);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Fines", "Admin");
        }

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
    
    public IActionResult FineCreate() //FINE CREATE
    {
        return View();
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

        return View(model);
    }


}