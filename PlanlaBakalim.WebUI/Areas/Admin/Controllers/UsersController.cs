using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using Microsoft.AspNetCore.Authorization;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class UsersController : Controller
    {
        private readonly DatabaseContext _context;

        public UsersController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalCount = await _context.Users.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var users = await _context.Users
                .Include(u => u.Appointments)
                .OrderByDescending(u => u.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users
                .Include(u => u.Appointments)
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });
                }

                user.IsActive = !user.IsActive;
                user.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return Json(new { success = true, isActive = user.IsActive, message = $"Kullanıcı {(user.IsActive ? "aktif" : "pasif")} edildi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İşlem sırasında hata oluştu." });
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                    user.CreatedDate = DateTime.Now;
                    user.IsActive = true;

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Kullanıcı başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kullanıcı oluşturulurken hata oluştu.";
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, User user)
        {
            try
            {
                if (id != user.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.Phone = user.Phone;
                    existingUser.Role = user.Role;
                    existingUser.IsActive = user.IsActive;
                    existingUser.UpdatedDate = DateTime.Now;

                    if (!string.IsNullOrEmpty(user.PasswordHash))
                    {
                        existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Kullanıcı başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kullanıcı güncellenirken hata oluştu.";
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Kullanıcı başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Kullanıcı silinirken hata oluştu." });
            }
        }
    }
}


