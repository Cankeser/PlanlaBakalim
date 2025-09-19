using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using PlanlaBakalim.Utilities;
using PlanlaBakalim.WebUI.Areas.Admin.ViewModels;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class BusinessesController : Controller
    {
        private readonly DatabaseContext _context;

        public BusinessesController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalCount = await _context.Businesses.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var businesses = await _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.Owner)
                .Include(b => b.BusinessAddress)
                    .ThenInclude(ba => ba.District)
                        .ThenInclude(d => d.City)
                .OrderByDescending(b => b.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(businesses);
        }

        public async Task<IActionResult> Details(int id)
        {
            var business = await _context.Businesses
                .Include(b => b.Category)
                .Include(b => b.Owner)
                .Include(b => b.BusinessAddress)
                    .ThenInclude(ba => ba.District)
                        .ThenInclude(d => d.City)
                .Include(b => b.WorkingHours)
                .Include(b => b.Services)
                .Include(b => b.Employees)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new BusinessCreateViewModel
            {
                Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync(),
                Users = await _context.Users.Where(u => u.IsActive).ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BusinessCreateViewModel viewModel, IFormFile profileImage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var business = new Business
                    {
                        Name = viewModel.Name,
                        PhoneNumber = viewModel.PhoneNumber,
                        Email = viewModel.Email,
                        Website = viewModel.Website,
                        Description = viewModel.Description,
                        CategoryId = viewModel.CategoryId,
                        OwnerId = viewModel.OwnerId,
                        IsActive = viewModel.IsActive,
                        AppointmentSlotDuration = viewModel.AppointmentSlotDuration,
                        CreatedDate = DateTime.Now
                    };

                    // Profil resmi yükleme
                    if (profileImage != null && profileImage.Length > 0)
                    {
                        var url = await FileHelper.FileLoaderAsync(profileImage, "/img/business");
                        if (!string.IsNullOrEmpty(url))
                            business.ProfileImageUrl = url;
                    }

                    _context.Businesses.Add(business);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "İşletme başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İşletme oluşturulurken hata oluştu.";
            }

            viewModel.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            viewModel.Users = await _context.Users.Where(u => u.IsActive).ToListAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var business = await _context.Businesses.FindAsync(id);
            if (business == null)
            {
                return NotFound();
            }

            var viewModel = new BusinessEditViewModel
            {
                Id = business.Id,
                Name = business.Name,
                ProfileImageUrl = business.ProfileImageUrl,
                PhoneNumber = business.PhoneNumber,
                Email = business.Email,
                Website = business.Website,
                Description = business.Description,
                CategoryId = business.CategoryId,
                OwnerId = business.OwnerId,
                IsActive = business.IsActive,
                AppointmentSlotDuration = business.AppointmentSlotDuration,
                Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync(),
                Users = await _context.Users.Where(u => u.IsActive).ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BusinessEditViewModel viewModel, IFormFile profileImage)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Mevcut işletmeyi al
                    var existingBusiness = await _context.Businesses.FindAsync(id);
                    if (existingBusiness == null)
                    {
                        return NotFound();
                    }

                    // Profil resmi güncelleme
                    if (profileImage != null && profileImage.Length > 0)
                    {
                        // Eski resmi sil
                        if (!string.IsNullOrEmpty(existingBusiness.ProfileImageUrl))
                        {
                            FileHelper.FileRemover(existingBusiness.ProfileImageUrl);
                        }

                        var url = await FileHelper.FileLoaderAsync(profileImage, "/img/business");
                        if (!string.IsNullOrEmpty(url))
                            existingBusiness.ProfileImageUrl = url;
                    }

                    // İşletme bilgilerini güncelle
                    existingBusiness.Name = viewModel.Name;
                    existingBusiness.PhoneNumber = viewModel.PhoneNumber;
                    existingBusiness.Email = viewModel.Email;
                    existingBusiness.Website = viewModel.Website;
                    existingBusiness.Description = viewModel.Description;
                    existingBusiness.CategoryId = viewModel.CategoryId;
                    existingBusiness.OwnerId = viewModel.OwnerId;
                    existingBusiness.IsActive = viewModel.IsActive;
                    existingBusiness.AppointmentSlotDuration = viewModel.AppointmentSlotDuration;
                    existingBusiness.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    TempData["Success"] = "İşletme başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İşletme güncellenirken hata oluştu.";
            }

            viewModel.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            viewModel.Users = await _context.Users.Where(u => u.IsActive).ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var business = await _context.Businesses.FindAsync(id);
                if (business == null)
                {
                    return Json(new { success = false, message = "İşletme bulunamadı." });
                }

                business.IsActive = !business.IsActive;
                business.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    isActive = business.IsActive,
                    message = business.IsActive ? "İşletme aktifleştirildi." : "İşletme pasifleştirildi."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İşlem sırasında hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var business = await _context.Businesses.FindAsync(id);
                if (business == null)
                {
                    return Json(new { success = false, message = "İşletme bulunamadı." });
                }

                _context.Businesses.Remove(business);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "İşletme başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İşletme silinirken hata oluştu." });
            }
        }
    }
}



