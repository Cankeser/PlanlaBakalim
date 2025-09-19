using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using System.Threading.Tasks;

namespace PlanlaBakalim.WebUI.Controllers
{
    public class FavoritesController : BaseController
    {
        private readonly IService<UserFavorites> _userFavorites;

        public FavoritesController(IService<UserFavorites> userFavorites)
        {
            _userFavorites = userFavorites;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int businessId)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Json(new { redirectUrl = Url.Action("LogIn", "Account") });
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Json(new { redirectUrl = Url.Action("LogIn", "Account") });

            var userId = int.Parse(userIdClaim.Value);

            var existingFavorite = await _userFavorites.GetAsync(uf => uf.UserId == userId && uf.BusinessId == businessId);
            if (existingFavorite != null)
                return Conflict("Zaten favorilerde");

            var favorite = new UserFavorites
            {
                UserId = userId,
                BusinessId = businessId
            };

            _userFavorites.Add(favorite);
            await _userFavorites.SaveChangesAsync();

            return Ok("Favorilere eklendi");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int businessId)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Json(new { redirectUrl = Url.Action("LogIn", "Account") });
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Json(new { redirectUrl = Url.Action("LogIn", "Account") });

            var userId = int.Parse(userIdClaim.Value);

            var favorite = await _userFavorites.GetAsync(uf => uf.UserId == userId && uf.BusinessId == businessId);
            if (favorite == null)
                return NotFound("Favori bulunamadı");

            _userFavorites.Delete(favorite);
            await _userFavorites.SaveChangesAsync();

            return Ok("Favorilerden çıkarıldı");
        }
    }
}
