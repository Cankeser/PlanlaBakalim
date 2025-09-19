using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class CategoriesController : Controller
    {
        private readonly DatabaseContext _context;

        public CategoriesController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalCount = await _context.Categories.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Businesses)
                .OrderBy(c => c.OrderNo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Businesses)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.IsActive = !category.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = category.IsActive });
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ParentCategories = _context.Categories.Where(c => c.IsActive && c.ParentId == null).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    category.CreatedDate = DateTime.Now;
                    category.IsActive = true;

                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Kategori başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kategori oluşturulurken hata oluştu.";
            }

            ViewBag.ParentCategories = await _context.Categories.Where(c => c.IsActive && c.ParentId == null).ToListAsync();
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            ViewBag.ParentCategories = await _context.Categories.Where(c => c.IsActive && c.ParentId == null && c.Id != id).ToListAsync();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            try
            {
                if (id != category.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var existingCategory = await _context.Categories.FindAsync(id);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }

                    existingCategory.Name = category.Name;
                    existingCategory.Description = category.Description;
                    existingCategory.ParentId = category.ParentId;
                    existingCategory.OrderNo = category.OrderNo;
                    existingCategory.IsActive = category.IsActive;
                    // UpdatedDate property doesn't exist in Category entity

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Kategori başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kategori güncellenirken hata oluştu.";
            }

            ViewBag.ParentCategories = await _context.Categories.Where(c => c.IsActive && c.ParentId == null && c.Id != id).ToListAsync();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return Json(new { success = false, message = "Kategori bulunamadı." });
                }

                // Check if category has businesses
                var hasBusinesses = await _context.Businesses.AnyAsync(b => b.CategoryId == id);
                if (hasBusinesses)
                {
                    return Json(new { success = false, message = "Bu kategoriye ait işletmeler bulunduğu için silinemez." });
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Kategori başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Kategori silinirken hata oluştu." });
            }
        }
    }
}


