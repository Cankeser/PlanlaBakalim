using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;

namespace PlanlaBakalim.WebUI.ViewComponents
{
    public class Categories : ViewComponent
    {
        private readonly IService<Category> _service;
        public Categories(IService<Category> service)
        {
            _service = service;
        }
        public IViewComponentResult Invoke(string mode = "desktop")
        {

            var categories= _service.Queryable()
                .GroupBy(c => c.ParentId ?? 0)
                .ToDictionary(g => g.Key, g => g.ToList());
            ViewData["Mode"] = mode;
            return View(categories);
        }
    }
}
