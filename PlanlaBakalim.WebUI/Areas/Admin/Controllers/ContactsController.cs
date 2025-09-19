using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class ContactsController : Controller
    {
        private readonly DatabaseContext _context;

        public ContactsController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? q = null, string filter = "all")
        {
            var query = _context.Contacts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(c => c.Name.Contains(q) || c.Email.Contains(q) || c.Subject.Contains(q));
            }

            if (filter == "unread") query = query.Where(c => !c.IsRead);
            if (filter == "read") query = query.Where(c => c.IsRead);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var contacts = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.Query = q;
            ViewBag.Filter = filter;

            return View(contacts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null) return NotFound();

            if (!contact.IsRead)
            {
                contact.IsRead = true;
                contact.ReadAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return View(contact);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleRead(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) return Json(new { success = false, message = "Mesaj bulunamadı." });

            contact.IsRead = !contact.IsRead;
            contact.ReadAt = contact.IsRead ? DateTime.Now : null;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isRead = contact.IsRead });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var contact = await _context.Contacts.FindAsync(id);
                if (contact == null)
                {
                    return Json(new { success = false, message = "Mesaj bulunamadı." });
                }

                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Mesaj silindi." });
            }
            catch
            {
                return Json(new { success = false, message = "Silme sırasında hata oluştu." });
            }
        }
    }
}


