using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class CitiesController : Controller
    {
        private readonly DatabaseContext _context;

        public CitiesController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalCount = await _context.Cities.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var cities = await _context.Cities
                .Include(c => c.Districts)
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(cities);
        }

        public async Task<IActionResult> Details(int id)
        {
            var city = await _context.Cities
                .Include(c => c.Districts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(City city)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    city.CreatedDate = DateTime.Now;

                    _context.Cities.Add(city);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "İl başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İl oluşturulurken hata oluştu.";
            }

            return View(city);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, City city)
        {
            try
            {
                if (id != city.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var existingCity = await _context.Cities.FindAsync(id);
                    if (existingCity == null)
                    {
                        return NotFound();
                    }

                    existingCity.Name = city.Name;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "İl başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İl güncellenirken hata oluştu.";
            }

            return View(city);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var city = await _context.Cities
                    .Include(c => c.Districts)
                    .FirstOrDefaultAsync(c => c.Id == id);
                
                if (city == null)
                {
                    return Json(new { success = false, message = "İl bulunamadı." });
                }

                // İlçeleri kontrol et
                if (city.Districts.Any())
                {
                    return Json(new { success = false, message = "Bu ile bağlı ilçeler bulunmaktadır. Önce ilçeleri siliniz." });
                }

                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "İl başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İl silinirken hata oluştu." });
            }
        }
    }
}
