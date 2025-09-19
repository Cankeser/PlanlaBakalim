using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.Utilities;
using System.Drawing;
using System.Threading.Tasks;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Controllers
{
    [Area("BusinessPanel")]
    [Authorize(AuthenticationSchemes = "BusinessScheme", Policy = "BusinessPolicy")]
    public class GalleryController : Controller
    {
        private readonly IService<Gallery> _galleryService;

        public GalleryController(IService<Gallery> galleryService)
        {
            _galleryService = galleryService;
        }

     
        public async Task<IActionResult> Index()
        {
            var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
            var galleries=await _galleryService.GetAllAsync(g=>g.BusinessId==businessId); 
            return View(galleries);
        }

        [HttpGet]
        public async Task<IActionResult> GetPhotoDetails(int id)
        {
            var photo = await _galleryService.FindAsync(id);
            if (photo == null) 
                return NotFound();
            return Json(new
            {
                Image = photo.ImageUrl,
                Title = photo.Title,
                IsVisibleProfile = photo.IsVisibleProfile
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var photo = await _galleryService.FindAsync(id);
            if (photo == null)
                return Json(new { success = false, message = "Fotoğraf bulunamadı" });

            _galleryService.Delete(photo);
            await _galleryService.SaveChangesAsync();
           var silindiMi= FileHelper.FileRemover(photo.ImageUrl);

            TempData["ToastMessage"] = "Fotoğraf başarıyla silindi";
            return RedirectToAction("Index"); 
        }

        [HttpPost]
        public async Task<IActionResult> ToggleProfileVisibility(int id, bool isVisible)
        {
            var photo = await _galleryService.FindAsync(id);
            if (photo == null)
                return Json(new { success = false, message = "Fotoğraf bulunamadı" });

            photo.IsVisibleProfile = isVisible;
            await _galleryService.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = isVisible
                ? "Fotoğraf artık profilde görünüyor"
                : "Fotoğraf profilden gizlendi"
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddPhoto(IFormFile file, string title, bool showProfile)
        {
            if (file == null)
                return Json(new { success = false, message = "Fotoğraf yüklenmedi" });

            var businessClaim = User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value;
            if (!int.TryParse(businessClaim, out var businessId))
                return Json(new { success = false, message = "Business bilgisi alınamadı" });

            try
            {
                var imageUrl = await FileHelper.FileLoaderAsync(file, "/img/business/");
                if (string.IsNullOrWhiteSpace(imageUrl))
                    return Json(new { success = false, message = "Fotoğraf yüklenemedi" });

                var gallery = new Gallery
                {
                    ImageUrl = imageUrl,
                    Title = string.IsNullOrWhiteSpace(title) ? "Başlık Yok" : title,
                    IsVisibleProfile = showProfile,
                    BusinessId = businessId
                };

                _galleryService.Add(gallery);
                await _galleryService.SaveChangesAsync();

                return Json(new { success = true, message = "Fotoğraf başarıyla eklendi" });
            }
            catch (DbUpdateException dbEx)
            {
                var innerMsg = dbEx.InnerException?.Message ?? dbEx.Message;
                return Json(new { success = false, message = "DB hatası: " + innerMsg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Sunucu hatası: " + ex.Message });
            }
        }


    }
}
