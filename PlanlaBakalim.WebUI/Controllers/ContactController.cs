using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using System.Threading.Tasks;

namespace PlanlaBakalim.WebUI.Controllers
{
    [Route("Iletisim")]
    [AllowAnonymous]
    public class ContactController : BaseController
    {
        private readonly IService<Contact> _service;

        public ContactController(IService<Contact> service)
        {
           _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Contact model)
        {
            if (!ModelState.IsValid)
            {
              return NotFound();
            }
            var contact = new Contact
            {
                Name = model.Name,
                Email = model.Email,
                Subject = model.Subject,
                Message = model.Message,
            };
            _service.Add(contact);
           var result= await _service.SaveChangesAsync();
            if (result>0)
            {
                ViewBag.Message = "Mesajınız Başarıyla Gönderildi";
                ModelState.Clear();
            }
            else
            {
                ViewBag.Message = "Mesajınız Gönderilirken Bir Hata Oluştu";
            }


            return View();
        }
    }
}
