using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class HomeController : Controller
    {
        private readonly DatabaseContext _context;

        public HomeController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Admin dashboard istatistikleri
            var totalUsers = await _context.Users.CountAsync();
            var totalBusinesses = await _context.Businesses.CountAsync();
            var totalAppointments = await _context.Appointments.CountAsync();
            var totalReviews = await _context.Reviews.CountAsync();

            // Son kayıt olan kullanıcılar (Admin hariç)
            var recentUsers = await _context.Users
                .Where(u => u.Role != PlanlaBakalim.Core.Enums.UserRole.Admin)
                .OrderByDescending(u => u.CreatedDate)
                .Take(5)
                .Select(u => new { u.FullName, u.CreatedDate })
                .ToListAsync();

            // Son eklenen işletmeler
            var recentBusinesses = await _context.Businesses
                .Include(b => b.Category)
                .OrderByDescending(b => b.CreatedDate)
                .Take(5)
                .Select(b => new { b.Name, b.CreatedDate, CategoryName = b.Category.Name })
                .ToListAsync();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalBusinesses = totalBusinesses;
            ViewBag.TotalAppointments = totalAppointments;
            ViewBag.TotalReviews = totalReviews;
            ViewBag.RecentUsers = recentUsers;
            ViewBag.RecentBusinesses = recentBusinesses;

            return View();
        }
    }
}


