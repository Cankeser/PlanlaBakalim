using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.Utilities;
using PlanlaBakalim.WebUI.Models;

namespace PlanlaBakalim.WebUI.Controllers
{
    public class RegisterController : Controller
    {
       private readonly IService<User> _userService;
        public RegisterController(IService<User> userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Index(UserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userService.GetAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                TempData["ErrorMessage"] = "Bu email adresi zaten kayıtlı.";
                return View(model);
            }


            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = PasswordHasher.Hash(model.Password),
                Phone = model.Phone,
            };
            _userService.Add(user);
            int count = await _userService.SaveChangesAsync();

            if (count > 0)
            {
                TempData["SuccessMessage"] = "Hesabınız başarıyla oluşturuldu! Lütfen giriş yapın.";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                TempData["ErrorMessage"] = "Kayıt işlemi sırasında bir hata oluştu. Lütfen tekrar deneyiniz.";
                return View(model);
            }
        }
    }
}