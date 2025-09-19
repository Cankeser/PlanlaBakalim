using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using System.Threading.Tasks;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Controllers
{
    [Area("BusinessPanel")]
    [Authorize(AuthenticationSchemes = "BusinessScheme", Policy = "BusinessPolicy")]
    public class WorkingHoursController : Controller
    {
        private readonly IService<BusinessWorkingHour> _workingHoursService;
        public WorkingHoursController(IService<BusinessWorkingHour> workingHoursService)
        {
            _workingHoursService = workingHoursService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
            var workingHours = await _workingHoursService.GetAllAsync(wh => wh.BusinessId == businessId);
            return View(workingHours);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateDay([FromBody]BusinessWorkingHour model)
        {
            var entity = await _workingHoursService.FindAsync(model.Id);
            if (entity == null)
                return Json(new { success = false, message = "Kayıt bulunamadı" });

            entity.IsOpen = model.IsOpen;
            entity.OpenTime = model.OpenTime;
            entity.CloseTime = model.CloseTime;

            _workingHoursService.Update(entity);
            await _workingHoursService.SaveChangesAsync();
            return Json(new { success = true, message = $"{entity.Day} günü güncellendi" });
        }


    }
}
