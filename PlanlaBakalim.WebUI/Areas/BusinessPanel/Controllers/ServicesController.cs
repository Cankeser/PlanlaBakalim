using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Core.DTOs;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.Utilities;
using System.Threading.Tasks;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Controllers
{
    [Area("BusinessPanel")]
    [Authorize(AuthenticationSchemes = "BusinessScheme", Policy = "BusinessPolicy")]
    public class ServicesController : Controller
    {
        private readonly IService<BusinessService> _businessServicesService;

        public ServicesController(IService<BusinessService> businessServicesService)
        {
            _businessServicesService = businessServicesService;
        }
        public async Task<IActionResult> Index()
        {
            var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
            var services = await _businessServicesService.GetAllAsync(s => s.BusinessId == businessId);
            return View(services);
        }

        public async Task<IActionResult> GetServiceDetails(int id)
        {
            var service = await _businessServicesService.GetAsync(s => s.Id == id);
            if (service == null)
                return Json(new { success = false, message = "Hizmet bulunamadı." });

            return Json(new { success = true, data = service });
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] BusinessService model)
        {
            if (model == null || model.Id <= 0)
                return Json(new { success = false, message = "Geçersiz veri." });

            var service = await _businessServicesService.GetAsync(s => s.Id == model.Id);
            if (service == null)
                return Json(new { success = false, message = "Hizmet bulunamadı." });

            service.Name = model.Name;
            service.Description = model.Description;
            service.Price = model.Price;
            service.IsActive = model.IsActive;

            _businessServicesService.Update(service);
            await _businessServicesService.SaveChangesAsync();
            return Json(new { success = true, message = "Hizmet güncellendi." });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ServiceDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Hizmet eklenemedi." });
            var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
            var service = new BusinessService
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                BusinessId=businessId
            };

            _businessServicesService.Add(service);
            await _businessServicesService.SaveChangesAsync();
            return Json(new { success = true, message = "Hizmet başarıyla eklendi." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var photo = await _businessServicesService.FindAsync(id);
            if (photo == null)
                return Json(new { success = false, message = "Hizmet bulunamadı" });

            _businessServicesService.Update(photo);
            await _businessServicesService.SaveChangesAsync();
     

            TempData["ToastMessage"] = "Hizmet başarıyla silindi";
            return RedirectToAction("Index");
        }

    }
}
