using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Data;
using PlanlaBakalim.Core.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PlanlaBakalim.Core.Enums;
using PlanlaBakalim.Utilities;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly DatabaseContext _context;

        public AuthController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated && User.Identity.AuthenticationType == "AdminScheme")
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "E-posta ve şifre gereklidir.";
                return View();
            }

            // Admin kullanıcısını kontrol et (Role = Admin olan kullanıcı)
            var admin = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Role == UserRole.Admin);

            if (admin == null)
            {
                TempData["Error"] = $"Admin kullanıcısı bulunamadı. Email: {email}";
                return View();
            }

            if (!admin.IsActive)
            {
                TempData["Error"] = "Admin kullanıcısı aktif değil.";
                return View();
            }

            // Şifre hash kontrolü
            if (!PasswordHasher.Verify(password, admin.PasswordHash))
            {
                TempData["Error"] = "Geçersiz kullanıcı adı veya şifre.";
                return View();
            }

            // Claims oluştur
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.FullName),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Role, admin.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "AdminScheme");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync("AdminScheme", 
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminScheme");
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}


