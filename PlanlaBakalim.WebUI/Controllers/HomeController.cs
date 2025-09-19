using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.WebUI.Models;

namespace PlanlaBakalim.WebUI.Controllers
{  
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly IService<Category> _categoryService;

        public HomeController( IService<Category> categoryService)
        {
            _categoryService = categoryService;
        }
     
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync(c => c.IsActive && c.ParentId == null);
            
            return View(categories);
        }

    }
}
