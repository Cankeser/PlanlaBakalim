using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class ReviewsController : Controller
    {
        private readonly DatabaseContext _context;

        public ReviewsController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalCount = await _context.Reviews.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Business)
                .OrderByDescending(r => r.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(reviews);
        }

        public async Task<IActionResult> Details(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Business)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleVisibility(int id)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);
                if (review == null)
                {
                    return Json(new { success = false, message = "Yorum bulunamadı." });
                }

                review.IsVisibleOnProfile = !review.IsVisibleOnProfile;
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    isVisible = review.IsVisibleOnProfile,
                    message = review.IsVisibleOnProfile ? "Yorum görünür yapıldı." : "Yorum gizlendi."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "İşlem sırasında hata oluştu." });
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Users = _context.Users.Where(u => u.IsActive).ToList();
            ViewBag.Businesses = _context.Businesses.Where(b => b.IsActive).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Review review)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    review.CreatedDate = DateTime.Now;
                    review.IsVisibleOnProfile = true;

                    _context.Reviews.Add(review);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Yorum başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Yorum oluşturulurken hata oluştu.";
            }

            ViewBag.Users = await _context.Users.Where(u => u.IsActive).ToListAsync();
            ViewBag.Businesses = await _context.Businesses.Where(b => b.IsActive).ToListAsync();
            return View(review);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            ViewBag.Users = await _context.Users.Where(u => u.IsActive).ToListAsync();
            ViewBag.Businesses = await _context.Businesses.Where(b => b.IsActive).ToListAsync();
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Review review)
        {
            try
            {
                if (id != review.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var existingReview = await _context.Reviews.FindAsync(id);
                    if (existingReview == null)
                    {
                        return NotFound();
                    }

                    existingReview.UserId = review.UserId;
                    existingReview.BusinessId = review.BusinessId;
                    existingReview.Rating = review.Rating;
                    existingReview.Comment = review.Comment;
                    existingReview.IsVisibleOnProfile = review.IsVisibleOnProfile;
                    // UpdatedDate property doesn't exist in Review entity

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Yorum başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Yorum güncellenirken hata oluştu.";
            }

            ViewBag.Users = await _context.Users.Where(u => u.IsActive).ToListAsync();
            ViewBag.Businesses = await _context.Businesses.Where(b => b.IsActive).ToListAsync();
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);
                if (review == null)
                {
                    return Json(new { success = false, message = "Yorum bulunamadı." });
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Yorum başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Yorum silinirken hata oluştu." });
            }
        }
    }
}


