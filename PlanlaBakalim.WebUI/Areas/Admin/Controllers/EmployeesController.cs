using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class EmployeesController : Controller
    {
        private readonly DatabaseContext _context;

        public EmployeesController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalCount = await _context.Employees.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var employees = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Business)
                .OrderByDescending(e => e.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _context.Users
                .Where(u => u.IsActive && u.Role == PlanlaBakalim.Core.Enums.UserRole.Musteri)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();
            
            ViewBag.Businesses = await _context.Businesses
                .Where(b => b.IsActive)
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    employee.CreatedDate = DateTime.Now;
                    employee.IsActive = true;
                    
                    _context.Employees.Add(employee);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Çalışan başarıyla eklendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Çalışan eklenirken hata oluştu.";
            }

            // ViewBag'leri tekrar yükle
            ViewBag.Users = await _context.Users
                .Where(u => u.IsActive && u.Role == PlanlaBakalim.Core.Enums.UserRole.Musteri)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();
            
            ViewBag.Businesses = await _context.Businesses
                .Where(b => b.IsActive)
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();

            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Business)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.Users = await _context.Users
                .Where(u => u.IsActive && u.Role == PlanlaBakalim.Core.Enums.UserRole.Musteri)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();
            
            ViewBag.Businesses = await _context.Businesses
                .Where(b => b.IsActive)
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            try
            {
                if (id != employee.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var existingEmployee = await _context.Employees.FindAsync(id);
                    if (existingEmployee == null)
                    {
                        return NotFound();
                    }

                    existingEmployee.UserId = employee.UserId;
                    existingEmployee.BusinessId = employee.BusinessId;
                    existingEmployee.Position = employee.Position;
                    existingEmployee.StartDate = employee.StartDate;
                    existingEmployee.IsActive = employee.IsActive;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Çalışan başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Çalışan güncellenirken hata oluştu.";
            }

            // ViewBag'leri tekrar yükle
            ViewBag.Users = await _context.Users
                .Where(u => u.IsActive && u.Role == PlanlaBakalim.Core.Enums.UserRole.Musteri)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();
            
            ViewBag.Businesses = await _context.Businesses
                .Where(b => b.IsActive)
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();

            return View(employee);
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Business)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Çalışan başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Çalışan silinirken hata oluştu.";
            }

            return RedirectToAction("Index");
        }
    }
}


