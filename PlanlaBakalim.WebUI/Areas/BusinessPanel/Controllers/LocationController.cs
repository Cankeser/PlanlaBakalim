using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Controllers
{
    [Area("BusinessPanel")]
    public class LocationController : Controller
    {
        private readonly IService<City> _cityService;
        private readonly IService<District> _districtService;

        public LocationController(IService<City> cityService, IService<District> districtService)
        {
            _cityService = cityService;
            _districtService = districtService;
        }
        public async Task<IActionResult> GetCities()
        {
            var cities = await _cityService.GetAllAsync();
            return Json(cities);
        }

        [HttpGet]
        public async Task<IActionResult> GetDistricts(int cityId)
        {
            var districts = await _districtService.Queryable()
                .Where(d => d.CityId == cityId)
                .ToListAsync();
            return Json(districts);
        }
    }
}
