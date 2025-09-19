using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class GalleryController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GalleryController(DatabaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalCount = await _context.Galleries.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var galleries = await _context.Galleries
                .Include(g => g.Business)
                .OrderByDescending(g => g.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(galleries);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Businesses = await _context.Businesses
                .Where(b => b.IsActive)
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Gallery gallery, IFormFile imageFile)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Resim yükleme
                    var fileName = await UploadImage(imageFile);
                    gallery.ImageUrl = fileName;
                }

                gallery.CreatedDate = DateTime.Now;
                gallery.IsActive = true;

                _context.Galleries.Add(gallery);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Galeri öğesi başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Galeri öğesi eklenirken hata oluştu.";
                ViewBag.Businesses = await _context.Businesses
                    .Where(b => b.IsActive)
                    .Select(b => new { b.Id, b.Name })
                    .ToListAsync();
                return View(gallery);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var gallery = await _context.Galleries.FindAsync(id);
            if (gallery == null)
            {
                return NotFound();
            }

            ViewBag.Businesses = await _context.Businesses
                .Where(b => b.IsActive)
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();

            return View(gallery);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Gallery gallery, IFormFile imageFile)
        {
            try
            {
                var existingGallery = await _context.Galleries.FindAsync(id);
                if (existingGallery == null)
                {
                    return NotFound();
                }

                // Yeni resim yüklendiyse
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Eski resmi sil
                    if (!string.IsNullOrEmpty(existingGallery.ImageUrl))
                    {
                        DeleteImage(existingGallery.ImageUrl);
                    }

                    // Yeni resmi yükle
                    var fileName = await UploadImage(imageFile);
                    existingGallery.ImageUrl = fileName;
                }

                existingGallery.Title = gallery.Title;
                existingGallery.Description = gallery.Description;
                existingGallery.BusinessId = gallery.BusinessId;
                existingGallery.IsActive = gallery.IsActive;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Galeri öğesi başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Galeri öğesi güncellenirken hata oluştu.";
                ViewBag.Businesses = await _context.Businesses
                    .Where(b => b.IsActive)
                    .Select(b => new { b.Id, b.Name })
                    .ToListAsync();
                return View(gallery);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var gallery = await _context.Galleries
                .Include(g => g.Business)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gallery == null)
            {
                return NotFound();
            }

            return View(gallery);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var gallery = await _context.Galleries.FindAsync(id);
                if (gallery == null)
                {
                    return NotFound();
                }

                // Resmi sil
                if (!string.IsNullOrEmpty(gallery.ImageUrl))
                {
                    DeleteImage(gallery.ImageUrl);
                }

                _context.Galleries.Remove(gallery);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Galeri öğesi başarıyla silindi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Galeri öğesi silinirken hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var gallery = await _context.Galleries.FindAsync(id);
                if (gallery == null)
                {
                    return NotFound();
                }

                gallery.IsActive = !gallery.IsActive;
                await _context.SaveChangesAsync();

                var statusText = gallery.IsActive ? "aktif" : "pasif";
                TempData["Success"] = $"Galeri öğesi {statusText} yapıldı.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Durum değiştirilirken hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                if (file == null || file.Length == 0)
                {
                    return Json(new { success = false, message = "Dosya seçilmedi." });
                }

                // Dosya tipini kontrol et
                if (!file.ContentType.StartsWith("image/"))
                {
                    return Json(new { success = false, message = "Sadece resim dosyası yüklenebilir." });
                }

                // Dosya boyutunu kontrol et (5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "Dosya boyutu 5MB'dan küçük olmalıdır." });
                }

                var fileName = await UploadImage(file);
                var filePath = $"/uploads/gallery/{fileName}";

                return Json(new { success = true, filePath = filePath, fileName = fileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Dosya yüklenirken hata oluştu." });
            }
        }

        private async Task<string> UploadImage(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "gallery");
            
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        private void DeleteImage(string fileName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "gallery", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ImportFromProject()
        {
            ViewBag.Businesses = await _context.Businesses
                .Where(b => b.IsActive)
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageUnusedImages()
        {
            try
            {
                // Veritabanındaki tüm resim yollarını al (sadece gerçekten kullanılan tablolardan)
                var userProfileImages = await _context.Users
                    .Select(u => u.ProfileUrl)
                    .Where(img => !string.IsNullOrEmpty(img))
                    .ToListAsync();
                
                var businessProfileImages = await _context.Businesses
                    .Select(b => b.ProfileImageUrl)
                    .Where(img => !string.IsNullOrEmpty(img))
                    .ToListAsync();
                
                // Tüm DB resimlerini birleştir
                var allDbImages = userProfileImages
                    .Concat(businessProfileImages)
                    .ToHashSet();

                // wwwroot klasöründeki tüm görselleri al (tüm alt klasörler dahil)
                var wwwrootPath = _webHostEnvironment.WebRootPath;
                var allImages = new List<string>();
                
                if (Directory.Exists(wwwrootPath))
                {
                    // Tüm alt klasörlerdeki resim dosyalarını bul
                    var imageExtensions = new[] { "*.jpg", "*.jpeg", "*.png", "*.gif", "*.bmp", "*.webp" };
                    
                    foreach (var extension in imageExtensions)
                    {
                        var files = Directory.GetFiles(wwwrootPath, extension, SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            // Dosya yolunu wwwroot'a göre relative path'e çevir
                            var relativePath = Path.GetRelativePath(wwwrootPath, file);
                            allImages.Add(relativePath);
                        }
                    }
                }

                // Kullanılmayan görseller (DB'de kayıtlı olmayan)
                var unusedImages = allImages.Where(img => !allDbImages.Contains(img)).ToList();
                
                // Kullanılan görseller (DB'de kayıtlı olan)
                var usedImages = allImages.Where(img => allDbImages.Contains(img)).ToList();

                // Debug bilgileri
                TempData["DebugInfo"] = $"Kullanıcı Profilleri: {userProfileImages.Count}, İşletme Profilleri: {businessProfileImages.Count}, Toplam DB: {allDbImages.Count}, Toplam Resim: {allImages.Count}, Kullanılmayan: {unusedImages.Count}, Kullanılan: {usedImages.Count}";

                ViewBag.UnusedImages = unusedImages;
                ViewBag.UsedImages = usedImages;
                ViewBag.TotalImages = allImages.Count;
                ViewBag.UnusedCount = unusedImages.Count;
                ViewBag.UsedCount = usedImages.Count;
                ViewBag.Businesses = await _context.Businesses
                    .Where(b => b.IsActive)
                    .Select(b => new { b.Id, b.Name })
                    .ToListAsync();

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Hata: {ex.Message}";
                return View();
            }
        }

        private bool IsImageFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" }.Contains(extension);
        }

        [HttpPost]
        public IActionResult DeleteUnusedImage(string imagePath)
        {
            try
            {
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    return Json(new { success = true, message = "Görsel başarıyla silindi." });
                }
                else
                {
                    return Json(new { success = false, message = "Görsel dosyası bulunamadı." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Görsel silinirken hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetBusinessProfileImage(string imagePath, int businessId)
        {
            try
            {
                var business = await _context.Businesses.FindAsync(businessId);
                if (business == null)
                {
                    return Json(new { success = false, message = "İşletme bulunamadı." });
                }

                // Dosyanın var olup olmadığını kontrol et
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);
                if (!System.IO.File.Exists(fullPath))
                {
                    return Json(new { success = false, message = "Görsel dosyası bulunamadı." });
                }

                // Eğer uploads/gallery klasöründe değilse, oraya kopyala
                string finalImageUrl = imagePath;
                if (!imagePath.StartsWith("uploads/gallery/"))
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "gallery");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imagePath)}";
                    var destPath = Path.Combine(uploadsFolder, fileName);
                    System.IO.File.Copy(fullPath, destPath);
                    finalImageUrl = fileName;
                }

                // İşletme profil resmini güncelle
                business.ProfileImageUrl = finalImageUrl;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "İşletme profil resmi başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Resim eklenirken hata oluştu." });
            }
        }

        [HttpPost]
        public IActionResult DeleteUnusedImage(string imageName, string source)
        {
            try
            {
                string filePath;
                
                if (source == "project")
                {
                    filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", imageName);
                }
                else
                {
                    filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "gallery", imageName);
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Json(new { success = true, message = "Dosya başarıyla silindi." });
                }
                else
                {
                    return Json(new { success = false, message = "Dosya bulunamadı." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Dosya silinirken hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromProject(int businessId, string[] selectedImages)
        {
            try
            {
                if (selectedImages == null || selectedImages.Length == 0)
                {
                    TempData["Error"] = "Lütfen en az bir resim seçin.";
                    return RedirectToAction("ImportFromProject");
                }

                var business = await _context.Businesses.FindAsync(businessId);
                if (business == null)
                {
                    TempData["Error"] = "İşletme bulunamadı.";
                    return RedirectToAction("ImportFromProject");
                }

                int importedCount = 0;
                foreach (var imageName in selectedImages)
                {
                    var sourcePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", imageName);
                    if (System.IO.File.Exists(sourcePath))
                    {
                        // Hedef klasörü oluştur
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "gallery");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Dosyayı kopyala
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageName)}";
                        var destPath = Path.Combine(uploadsFolder, fileName);
                        System.IO.File.Copy(sourcePath, destPath);

                        // Galeri kaydı oluştur
                        var gallery = new Gallery
                        {
                            Title = Path.GetFileNameWithoutExtension(imageName),
                            Description = $"Projeden içe aktarılan resim: {imageName}",
                            ImageUrl = fileName,
                            BusinessId = businessId,
                            IsActive = true,
                            IsVisibleProfile = true,
                            CreatedDate = DateTime.Now
                        };

                        _context.Galleries.Add(gallery);
                        importedCount++;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"{importedCount} resim başarıyla galeriye eklendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Resimler içe aktarılırken hata oluştu.";
                return RedirectToAction("ImportFromProject");
            }
        }
    }
}


