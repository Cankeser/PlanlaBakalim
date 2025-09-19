using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using System.Threading.Tasks;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Controllers
{
    [Area("BusinessPanel")]
    [Authorize(AuthenticationSchemes = "BusinessScheme", Policy = "BusinessPolicy")]
    public class SettingsController : Controller
    {
        private readonly IBusinessService _businessService;
        private readonly int businessId;

        public SettingsController(IBusinessService businessService)
        {
            _businessService = businessService;
        }
        public async Task<IActionResult> Index()
        {
            var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
            var business = await _businessService.GetAsync(b=>b.Id==businessId&&b.IsActive);
            if (business == null)
                return RedirectToAction("Logout", "Account", new { area = "BusinessPanel" });
            return View(business);
        }
    }
}
