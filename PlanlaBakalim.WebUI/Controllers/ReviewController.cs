using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using PlanlaBakalim.Core.DTOs;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.WebUI.Models;

namespace PlanlaBakalim.WebUI.Controllers
{
    [Route("Yorum")]
    public class ReviewController : BaseController
    {
        private readonly IService<Review> _reviewService;

        public ReviewController(IService<Review> reviewService)
        {
            _reviewService = reviewService;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [Route("Ekle")]
        public async Task<IActionResult> AddRating([FromBody] ReviewDto model)
        {
 
            if (model == null || model.BusinessId <= 0 || model.Rating < 1 || model.Rating > 5)
            {
                return BadRequest(new { message = "Geçersiz veri gönderildi." });
            }
            var userId = 0;
            if (User.Identity.IsAuthenticated)
            {
                userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
            }
    
            var rating = new Review
            {
                BusinessId = model.BusinessId,
                Rating = (byte)model.Rating,
                Comment = model.Comment,
                UserId =userId
            };

          _reviewService.Add(rating);
            await _reviewService.SaveChangesAsync();

            return Ok(new { message = "Değerlendirmeniz başarıyla kaydedildi." });
        }
    }
}
