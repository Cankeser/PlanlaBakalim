using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Controllers
{
    public class ContactController : Controller
    {
        private readonly DatabaseContext _context;

        public ContactController(DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Contact model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.IsRead = false;
                
                _context.Contacts.Add(model);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Mesajınız başarıyla gönderildi!";
                return RedirectToAction("Thanks");
            }
            
            return View(model);
        }

       
    }
}
