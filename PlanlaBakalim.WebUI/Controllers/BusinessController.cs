using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.WebUI.Models;

namespace PlanlaBakalim.WebUI.Controllers
{
    [Route("Isletmeler")]
    [AllowAnonymous]
    public class BusinessController : BaseController
    {
        private readonly IBusinessService _businessService;
        private readonly IService<BusinessService> _businessServiceService;
        private readonly IService<Employee> _employeeService;

        public BusinessController(IBusinessService businessService,  IService<BusinessService> businessServiceService, IService<Employee> employeeService)
        {
            _businessService = businessService;
            _businessServiceService = businessServiceService;
            _employeeService = employeeService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        [Route("{name}/{id}")]
        public async Task<IActionResult> BusinessDetail(int id, string name)
        {

            var business = await _businessService.Queryable()
                    .Include(b => b.WorkingHours)
                    .Include(b => b.Galleries)
                    .Include(b => b.Category)
                    .Include(b => b.Reviews)
                     .ThenInclude(r => r.User)
                    .FirstOrDefaultAsync(b => b.Id == id&&b.IsActive);
            if (business == null)
                return NotFound();
            var galeries = business.Galleries.Where(g => g.IsVisibleProfile).ToList();
            var review=business.Reviews.Where(r=>r.IsVisibleOnProfile).ToList();

            var model = new BusinessDetailVM
            {
                Business = new BusinessVM
                {
                    Id = business.Id,
                    Name = business.Name,
                    ProfileImageUrl = business.ProfileImageUrl,
                    CategoryName = business.Category.Name,
                    ReviewCount = business.Reviews.Count(),
                    AverageRating = business.Reviews.Any() ? business.Reviews.Average(r => r.Rating) : 0,
                    IsOpenNow = _businessService.IsOpenNow(business)
                },
                PhoneNumber = business?.PhoneNumber,
                Website = business?.Website,
                Description = business?.Description,
                BusinessWorkingHours = business.WorkingHours.Where(bw => bw.BusinessId == business.Id).ToList(),
                Reviews = review,
                Services = await _businessServiceService.GetAllAsync(s => s.BusinessId == id),
                Galleries = galeries,
                Employees= await _employeeService.Queryable().Include(e => e.User).Where(e => e.BusinessId == id && e.IsActive).ToListAsync()
            };

            return View(model);
        }    
    }
}
