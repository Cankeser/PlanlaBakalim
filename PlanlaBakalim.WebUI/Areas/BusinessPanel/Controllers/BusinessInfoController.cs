using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.Utilities;
using PlanlaBakalim.WebUI.Areas.BusinessPanel.Models;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Controllers
{
    [Area("BusinessPanel")]
    [Authorize(AuthenticationSchemes = "BusinessScheme", Policy = "BusinessPolicy")]
    public class BusinessInfoController : Controller
    {
        private readonly IBusinessService _businessService;

        public BusinessInfoController(IBusinessService businessService)
        {
            _businessService = businessService;
        }
        public async Task<IActionResult> Index()
        {
            var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
            var business = await _businessService.Queryable()
                .Include(x => x.BusinessAddress)
                .ThenInclude(x => x.District)
                .ThenInclude(x => x.City)
                .FirstOrDefaultAsync(x => x.Id == businessId&&x.IsActive);
            if (business == null)
                return RedirectToAction("Logout", "Account", new { area = "BusinessPanel" });
            var model = new BusinessUpdateVM
            {
                Id = business.Id,
                Name = business.Name,
                Phone = business.PhoneNumber,
                Email = business.Email,
                Website = business.Website,
                Description = business.Description,
                Address = business.BusinessAddress.StreetAddress,
                DistrictId = business.BusinessAddress.DistrictId,
                CityId = business.BusinessAddress.District.CityId,
                ProfileImageUrl = business.ProfileImageUrl,
                AppointmentSlotDuration = business.AppointmentSlotDuration,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BusinessUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            var business = await _businessService.Queryable()
                .Include(x => x.BusinessAddress)
                .ThenInclude(x => x.District)
                .ThenInclude(x => x.City)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (business == null)
                return RedirectToAction("Index", "Settings");

            business.Name = model.Name;
            business.PhoneNumber = model.Phone;
            business.Email = model.Email;
            business.Website = model.Website;
            business.Description = model.Description;
            business.AppointmentSlotDuration = model.AppointmentSlotDuration;
            business.BusinessAddress.StreetAddress = model.Address;
            business.BusinessAddress.DistrictId = model.DistrictId;

            if (model.Photo != null)
            {
                if (!string.IsNullOrEmpty(business.ProfileImageUrl))
                {
                    FileHelper.FileRemover(business.ProfileImageUrl);
                }
                business.ProfileImageUrl = await FileHelper.FileLoaderAsync(model.Photo);

                if (!string.IsNullOrEmpty(model.ProfileImageUrl))
                {
                    // eski resmi sil
                    FileHelper.FileRemover(model.ProfileImageUrl);
                }
            }

            _businessService.Update(business);
            await _businessService.SaveChangesAsync();

            return RedirectToAction("Index", "Settings");
        }
    }
}
