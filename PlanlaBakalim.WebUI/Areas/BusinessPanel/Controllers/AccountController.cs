using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.Utilities;
using PlanlaBakalim.WebUI.Areas.BusinessPanel.Models;
using PlanlaBakalim.WebUI.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Controllers
{
    [Area("BusinessPanel")]
    
    public class AccountController : Controller
    {
        private readonly IService<User> _userService;
        private readonly IBusinessService _businessService;
        private readonly IService<BusinessAdress> _businessAdressService;
        private readonly IService<BusinessWorkingHour> _businessWorkingHourService;
        private readonly IService<Category> _categoryService;
        private readonly IService<Employee> _employeeService;
        private readonly IService<BusinessService> _businessServicesService;

        public AccountController(IService<User> userService, IBusinessService businessService, IService<BusinessAdress> businessAdressService,
            IService<BusinessWorkingHour> businessWorkingHourService, IService<Category> categoryService, IService<Employee> employeeService, IService<BusinessService> businessServicesService)
        {
            _userService = userService;
            _businessService = businessService;
            _businessAdressService = businessAdressService;
            _businessWorkingHourService = businessWorkingHourService;
            _categoryService = categoryService;
            _employeeService = employeeService;
            _businessServicesService = businessServicesService;
        }

        [HttpGet]
        public async Task<IActionResult> LogIn()
        {
            var result = await HttpContext.AuthenticateAsync("BusinessScheme");
            if (result.Succeeded && result.Principal != null)
            {
                return RedirectToAction("Index", "Home", new { area = "BusinessPanel" });
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.GetAsync(u => u.Email == model.Email && u.Role == UserRole.IsletmeSahibi);
            if (user == null || !PasswordHasher.Verify(model.Password, user.PasswordHash))
            {
                TempData["MessageError"] = "Geçersiz e-posta veya şifre.";
                return View(model);
            }

            var business = await _businessService.GetAsync(b => b.OwnerId == user.Id&&b.IsActive);
            if (business == null)
            {
                TempData["MessageError"] = "İşletme hesabınız bulunamadı veya aktif değil.";
                return View(model);
            }

            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Name, user.FullName),
               new Claim(ClaimTypes.Email, user.Email),
               new Claim(ClaimTypes.Role, user.Role.ToString()),
               new Claim("BusinessId", business.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, "BusinessScheme");
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe, 
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(1) : (DateTimeOffset?)null
            };

            await HttpContext.SignInAsync("BusinessScheme", principal, authProperties);

            // ReturnUrl varsa oraya, yoksa BusinessPanel ana sayfaya
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home", new { area = "BusinessPanel" });
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var result = await HttpContext.AuthenticateAsync("BusinessScheme");
            if (result.Succeeded && result.Principal != null)
            {
                return RedirectToAction("Index", "Home", new { area = "BusinessPanel" });
            }

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterBusinessVM model)
        {

            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllAsync();
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
                return View();
            }
            var user = new User
            {
                FullName = model.OwnerName,
                Email = model.OwnerEmail,
                Phone = model.OwnerPhone,
                PasswordHash = PasswordHasher.Hash(model.Password),
                Role = UserRole.IsletmeSahibi
            };

            _userService.Add(user);

            var business = new Business
            {
                Name = model.BusinessName,
                Email = model.BusinessEmail,
                PhoneNumber = model.BusinessPhone,
                Description = model.BusinessDescription,
                Website = model.BusinessWebsite,
                CategoryId = model.CategoryId,
                AppointmentSlotDuration = model.AppointmentSlotDuration,
                OwnerId = user.Id,
                Owner = user
            };
            _businessService.Add(business);

            var businessService=new BusinessService
            {
                Business = business,
                BusinessId = business.Id,
                Name = "Genel Hizmet",
                Description = "Varsayılan hizmet",
                Price = 0
            };
            _businessServicesService.Add(businessService);
            var employee = new Employee
            {
                Business = business,
                BusinessId = business.Id,
                User = user,
                UserId = user.Id,
                IsActive = true,
                StartDate = DateTime.Now,
                Position = "İşletme Sahibi"
            };
            _employeeService.Add(employee);
          

            var businessAddress = new BusinessAdress
            {
                BusinessId = business.Id,
                DistrictId = model.BusinessDistrict,
                StreetAddress = model.BusinessAddress,
                Business = business

            };
            _businessAdressService.Add(businessAddress);


            foreach (var wh in model.WorkingHours)
            {
                var hour = new BusinessWorkingHour
                {
                    BusinessId = business.Id,
                    Business = business,
                    Day = wh.Day,
                    OpenTime = wh.OpenTime,
                    CloseTime = wh.CloseTime,
                    IsOpen = wh.IsOpen

                };
                _businessWorkingHourService.Add(hour);
            }
            try
            {
                // Tüm değişiklikleri tek bir işlemde kaydediyoruz
                int count = await _businessService.SaveChangesAsync();

                if (count > 0)
                {
                    TempData["MessageSuccess"] = "İşletme hesabınız başarıyla oluşturuldu! Lütfen giriş yapın.";
                    return RedirectToAction("LogIn", "Account", new { area = "BusinessPanel" });
                }
                else
                {
                    // Kayıt başarısızsa hata mesajı ekle ve sayfayı yeniden göster
                    ModelState.AddModelError("", "Kayıt işlemi sırasında bir hata oluştu. Lütfen tekrar deneyiniz.");
                    var categories = await _categoryService.GetAllAsync();
                    ViewBag.Categories = categories.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajını ekle ve sayfayı yeniden göster
                ModelState.AddModelError("", "Kayıt işlemi sırasında bir hata oluştu: " + ex.Message);
                var categories = await _categoryService.GetAllAsync();
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
                return View(model);
            }

        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync("BusinessScheme");
            return RedirectToAction("LogIn", "Account", new { area = "BusinessPanel" });

        }


    }
}
