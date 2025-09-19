using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.WebUI.Models;

namespace PlanlaBakalim.WebUI.ViewComponents
{
    public class BusinessListViewComponent : ViewComponent
    {
        private readonly IBusinessService _businessService;
        private readonly IService<UserFavorites> _userFavorites;

        public BusinessListViewComponent(IBusinessService businessService, IService<UserFavorites> userFavorites)
        {
            _businessService = businessService;
            _userFavorites = userFavorites;
        }

        public async Task<IViewComponentResult> InvokeAsync(int take=12)
        {
            var isFavorite = false;
            var user = ViewContext.HttpContext.User;
            int userId = 0;
            if (user.Identity.IsAuthenticated)
            {
                var userIdClaim = user.FindFirst("UserId");
                if (userIdClaim != null)
                {
                    int.TryParse(userIdClaim.Value, out  userId);
                }
            }

           

            var businesses = await _businessService.Queryable()
                .Where(b => b.IsActive)
                .Take(take)
                .Select(b => new BusinessVM
                {
                    Id = b.Id,
                    Name = b.Name,
                    ProfileImageUrl = b.ProfileImageUrl,
                    Address = b.BusinessAddress != null
                        ? $"{b.BusinessAddress.District.Name}, {b.BusinessAddress.District.City.Name}"
                        : "",
                    WorkingHours = b.WorkingHours != null && b.WorkingHours.Any()
                        ? $"{b.WorkingHours.Min(w => w.OpenTime):hh\\:mm} - {b.WorkingHours.Max(w => w.CloseTime):hh\\:mm}"
                        : "",
                    CategoryName = b.Category.Name,
                    ReviewCount = b.Reviews.Count(),
                    AverageRating = b.Reviews.Select(r => (double?)r.Rating).Average() ?? 0,
                    IsOpenNow = _businessService.IsOpenNow(b),
                    IsFavorited =_userFavorites.Queryable().Any(uf => uf.BusinessId == b.Id && uf.UserId ==userId)
                }).ToListAsync();

           

            return View(businesses); 
        }
    }

}
