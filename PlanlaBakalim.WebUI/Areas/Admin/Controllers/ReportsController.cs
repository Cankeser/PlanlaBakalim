using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class ReportsController : Controller
    {
        private readonly DatabaseContext _context;

        public ReportsController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reportData = new
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalBusinesses = await _context.Businesses.CountAsync(),
                TotalAppointments = await _context.Appointments.CountAsync(),
                TotalReviews = await _context.Reviews.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(u => u.IsActive),
                ActiveBusinesses = await _context.Businesses.CountAsync(b => b.IsActive),
                MonthlyAppointments = await _context.Appointments
                    .CountAsync(a => a.CreatedDate.Month == DateTime.Now.Month),
                MonthlyReviews = await _context.Reviews
                    .CountAsync(r => r.CreatedDate.Month == DateTime.Now.Month),
                AverageRating = await _context.Reviews.AverageAsync(r => (double?)r.Rating) ?? 0,
                TopCategories = await _context.Categories
                    .Include(c => c.Businesses)
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.Businesses.Count)
                    .Take(5)
                    .Select(c => new { c.Name, Count = c.Businesses.Count })
                    .ToListAsync(),
                RecentUsers = await _context.Users
                    .OrderByDescending(u => u.CreatedDate)
                    .Take(10)
                    .ToListAsync(),
                RecentBusinesses = await _context.Businesses
                    .Include(b => b.Category)
                    .OrderByDescending(b => b.CreatedDate)
                    .Take(10)
                    .ToListAsync()
            };

            return View(reportData);
        }

        public async Task<IActionResult> Export(string format = "pdf")
        {
            // Export functionality can be implemented here
            return Json(new { message = "Export functionality will be implemented" });
        }
    }
}


