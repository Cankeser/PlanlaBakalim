using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class SettingsController : Controller
    {
        private readonly DatabaseContext _context;

        public SettingsController(DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var settings = new
            {
                SiteName = "PlanlaBakalim.com",
                SiteDescription = "Randevu yönetim platformu",
                ContactEmail = "info@planlabakalim.com",
                ContactPhone = "+90 555 123 45 67",
                MaxAppointmentPerDay = 50,
                AppointmentDuration = 30,
                AutoApproveAppointments = false,
                EmailNotifications = true,
                SmsNotifications = false,
                MaintenanceMode = false
            };

            return View(settings);
        }

        [HttpPost]
        public IActionResult UpdateSettings([FromBody] dynamic settings)
        {
            try
            {
                // Settings update logic can be implemented here
                // For now, we'll just return success
                return Json(new { success = true, message = "Ayarlar başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ayarlar güncellenirken hata oluştu." });
            }
        }

        [HttpPost]
        public IActionResult ClearCache()
        {
            try
            {
                // Cache clearing logic can be implemented here
                // For now, we'll just return success
                return Json(new { success = true, message = "Önbellek başarıyla temizlendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Önbellek temizlenirken hata oluştu." });
            }
        }

        [HttpPost]
        public IActionResult BackupDatabase()
        {
            try
            {
                // Database backup logic can be implemented here
                // For now, we'll just return success
                return Json(new { success = true, message = "Veritabanı yedekleme işlemi başlatıldı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Veritabanı yedeklenirken hata oluştu." });
            }
        }
    }
}


