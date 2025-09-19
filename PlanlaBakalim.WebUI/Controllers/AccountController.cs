using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Core.DTOs;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.Utilities;
using PlanlaBakalim.WebUI.Models;
using System.Security.Claims;

namespace PlanlaBakalim.WebUI.Controllers
{
    [Route("Hesap")]

    public class AccountController : BaseController
    {
        private readonly IService<User> _userService;
        public AccountController(IService<User> userService)
        {
            _userService = userService;
        }
        [AllowAnonymous]
        [Route("GirisYap")]
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }
        [Route("GirisYap")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userService.GetAsync(u => u.Email == model.Email && u.IsActive);
            if (user == null || !PasswordHasher.Verify(model.Password, user.PasswordHash))
            {
                TempData["ErrorMessage"] = "Geçersiz e-posta veya şifre.";
                return View(model);
            }

            var claims = new List<Claim>
            {
               new Claim("UserId", user.Id.ToString()),
               new Claim(ClaimTypes.Name, user.FullName),
               new Claim(ClaimTypes.Email, user.Email),
               new Claim(ClaimTypes.Role,"Musteri"),
               new Claim("Avatar", user.ProfileUrl ?? "/img/userProfile.jpg")
            };

            var identity = new ClaimsIdentity(claims, "CustomerScheme");
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(1) : (DateTimeOffset?)null
            };

            await HttpContext.SignInAsync("CustomerScheme", principal, authProperties);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);


            return RedirectToAction("Index", "Home");
        }


        [Route("CikisYap")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync("CustomerScheme");
            return RedirectToAction("LogIn", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(UserVM model)
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

        [HttpPost]
        [Route("AjaxLogIn")]
        [AllowAnonymous]
        public async Task<IActionResult> AjaxLogIn([FromBody] LoginVM model)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = "Form geçersiz" });
            var user = await _userService.GetAsync(u => u.Email == model.Email && u.IsActive);
            if (user == null || !PasswordHasher.Verify(model.Password, user.PasswordHash))
                return Json(new { success = false, message = "Geçersiz e-posta veya şifre." });

            var claims = new List<Claim>
            {
              new Claim("UserId", user.Id.ToString()),
              new Claim(ClaimTypes.Name, user.FullName),
              new Claim(ClaimTypes.Email, user.Email),
              new Claim(ClaimTypes.Role,"Customer"),
             new Claim("Avatar", user.ProfileUrl ?? "/img/userProfile.jpg")
            };

            var identity = new ClaimsIdentity(claims, "CustomerScheme");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("CustomerScheme", principal);

            return Json(new { success = true, userId = user.Id, fullName = user.FullName });
        }

        [HttpPost]
        [Route("ProfilFotoGuncelle")]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Geçerli bir dosya seçin." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                return BadRequest(new { message = "Sadece resim dosyaları yüklenebilir." });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "Dosya boyutu 5MB'dan küçük olmalıdır." });

            var userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
            var user = await _userService.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            user.ProfileUrl = await FileHelper.FileLoaderAsync(file, "/img/profiles/");
            _userService.Update(user);
            await _userService.SaveChangesAsync();

            return Ok(new { message = "Profil fotoğrafı başarıyla güncellendi.", profileUrl = user.ProfileUrl });
        }

        [HttpPost]
        [Route("SifreDegistir")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Kullanıcı bilgisi bulunamadı." });

            int userId = int.Parse(userIdClaim);
            var user = await _userService.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest(new { message = "Yeni şifre ve onay şifresi eşleşmiyor." });

            if (!PasswordHasher.Verify(dto.CurrentPassword, user.PasswordHash))
                return BadRequest(new { message = "Mevcut şifre yanlış." });


            user.PasswordHash = PasswordHasher.Hash(dto.NewPassword);
            _userService.Update(user);
            await _userService.SaveChangesAsync();

            return Ok(new { message = "Şifre başarıyla güncellendi." });
        }

        [HttpPost]
        [Route("ProfilGuncelle")]
        public async Task<IActionResult> Update([FromBody] Dictionary<string, string> data)
        {
            var fullName = data["fullName"];
            var email = data["email"];
            var phone = data["phone"];

            int UserId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
            var user = await _userService.FindAsync(UserId);
            user.FullName = fullName;
            user.Email = email;
            user.Phone = phone;
            _userService.Update(user);
            await _userService.SaveChangesAsync();
            return Ok(new { message = "Profil başarıyla güncellendi." });
        }

        [HttpPost]
        [Route("HesapSil")]
        public async Task<IActionResult> DeleteAccount()
        {

            int UserId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
            var user = await _userService.FindAsync(UserId);

            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            }
            user.IsActive = false;
            _userService.Update(user);
            await _userService.SaveChangesAsync();
            await HttpContext.SignOutAsync("CustomerScheme");

            return Ok(new { message = "Hesap başarıyla silindi." });
        }

    }

}
