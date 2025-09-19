using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.Utilities;
using PlanlaBakalim.WebUI.Areas.BusinessPanel.Models;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Controllers
{
    [Area("BusinessPanel")]
    [Authorize(AuthenticationSchemes = "BusinessScheme", Policy = "BusinessPolicy")]
    public class EmployeeController : Controller
    {
        private readonly IService<Employee> _employeeService;
        private readonly IService<User> _userService;
        public EmployeeController(IService<Employee> employeeService, IService<User> userService)
        {
            _employeeService = employeeService;
            _userService = userService;
        }
        public IActionResult Index()
        {
            var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
            var employees = _employeeService.Queryable()
                .Include(e => e.User)
                .Where(e => e.BusinessId ==businessId &&e.IsActive)
                .ToList(); 

            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    ProfileUrl = model.Photo != null ? await FileHelper.FileLoaderAsync(model.Photo) : null,
                    Role = UserRole.Calisan
                };
                _userService.Add(user);
                await _userService.SaveChangesAsync();
                var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
                var employee = new Employee
                {
                    BusinessId = businessId,
                    UserId = user.Id,
                    StartDate = model.StartDate,
                    Position = model.Position
                };
                 _employeeService.Add(employee);
                await _employeeService.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result= await _employeeService.Queryable().Include(e => e.User).Where(e => e.Id == id).FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }
            var employee = new EmployeeVM
            {
                UserId = result.UserId,
                BusinessId = result.BusinessId,
                FullName = result.User.FullName,
                Email = result.User.Email,
                Phone = result.User.Phone,
                Position = result.Position,
                StartDate = result.StartDate,
                ProfileUrl = result.User.ProfileUrl

            };
            return View(employee);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeVM model)
        {
            if (ModelState.IsValid)
            {
                var employee = await _employeeService.Queryable().Include(e => e.User).Where(e => e.Id == model.Id).FirstOrDefaultAsync();
                if (employee == null)
                {
                    return NotFound();
                }
                employee.User.FullName = model.FullName;
                employee.User.Email = model.Email;
                employee.User.Phone = model.Phone;
                if (model.Photo != null)
                {
                    if (!string.IsNullOrEmpty(employee.User.ProfileUrl))
                    {
                        FileHelper.FileRemover(employee.User.ProfileUrl);
                    }
                    employee.User.ProfileUrl = await FileHelper.FileLoaderAsync(model.Photo);
                }
                employee.Position = model.Position;
                _employeeService.Update(employee);
                await _employeeService.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.Queryable().Include(e => e.User).Where(e => e.Id == id).FirstOrDefaultAsync();
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Employee model)
        {
            var employee = await _employeeService.FindAsync(model.Id);
            if (employee == null)
            {
                return NotFound();
            }
            employee.IsActive = false;
            employee.EndDate = DateTime.Now;
            _employeeService.Update(employee);
            await _employeeService.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
