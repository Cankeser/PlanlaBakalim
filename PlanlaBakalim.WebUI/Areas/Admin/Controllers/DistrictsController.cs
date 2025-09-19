using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class DistrictsController : Controller
    {
        private readonly DatabaseContext _context;

        public DistrictsController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, int? cityId = null)
        {
            var query = _context.Districts.Include(d => d.City).AsQueryable();
            
            if (cityId.HasValue)
            {
                query = query.Where(d => d.CityId == cityId.Value);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var districts = await query
                .OrderBy(d => d.City.Name)
                .ThenBy(d => d.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.CityId = cityId;
            ViewBag.Cities = await _context.Cities.OrderBy(c => c.Name).ToListAsync();

            return View(districts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var district = await _context.Districts
                .Include(d => d.City)
                .Include(d => d.BusinessAdresses)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (district == null)
            {
                return NotFound();
            }

            return View(district);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Cities = await _context.Cities.OrderBy(c => c.Name).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(District district)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    district.CreatedDate = DateTime.Now;

                    _context.Districts.Add(district);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "İlçe başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İlçe oluşturulurken hata oluştu.";
            }

            ViewBag.Cities = await _context.Cities.OrderBy(c => c.Name).ToListAsync();
            return View(district);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var district = await _context.Districts.FindAsync(id);
            if (district == null)
            {
                return NotFound();
            }

            ViewBag.Cities = await _context.Cities.OrderBy(c => c.Name).ToListAsync();
            return View(district);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, District district)
        {
            try
            {
                if (id != district.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var existingDistrict = await _context.Districts.FindAsync(id);
                    if (existingDistrict == null)
                    {
                        return NotFound();
                    }

                    existingDistrict.Name = district.Name;
                    existingDistrict.CityId = district.CityId;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "İlçe başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İlçe güncellenirken hata oluştu.";
            }

            ViewBag.Cities = await _context.Cities.OrderBy(c => c.Name).ToListAsync();
            return View(district);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var district = await _context.Districts
                    .Include(d => d.BusinessAdresses)
                    .FirstOrDefaultAsync(d => d.Id == id);
                
                if (district == null)
                {
                    return Json(new { success = false, message = "İlçe bulunamadı." });
                }

                // İşletme adreslerini kontrol et
                if (district.BusinessAdresses.Any())
                {
                    return Json(new { success = false, message = "Bu ilçeye bağlı işletme adresleri bulunmaktadır. Önce işletme adreslerini siliniz." });
                }

                _context.Districts.Remove(district);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "İlçe başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İlçe silinirken hata oluştu." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDistrictsByCity(int cityId)
        {
            var districts = await _context.Districts
                .Where(d => d.CityId == cityId)
                .OrderBy(d => d.Name)
                .Select(d => new { d.Id, d.Name })
                .ToListAsync();

            return Json(districts);
        }
    }
}
