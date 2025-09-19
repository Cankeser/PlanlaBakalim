using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class AppointmentsController : Controller
    {
        private readonly DatabaseContext _context;

        public AppointmentsController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalCount = await _context.Appointments.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var appointments = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Business)
                    .ThenInclude(b => b.Category)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.User)
                .OrderByDescending(a => a.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(appointments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Business)
                    .ThenInclude(b => b.Category)
                .Include(a => a.Employee)
                    .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = (PlanlaBakalim.Core.Enums.AppointmentStatus)status;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _context.Users
                .Where(u => u.IsActive)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();
            
            ViewBag.Businesses = await _context.Businesses
                .Where(b => b.IsActive)
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();
            
            ViewBag.Employees = await _context.Employees
                .Include(e => e.User)
                .Where(e => e.IsActive)
                .Select(e => new { e.Id, Name = e.User.FullName })
                .ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    appointment.CreatedDate = DateTime.Now;
                    appointment.Status = PlanlaBakalim.Core.Enums.AppointmentStatus.Pending;

                    _context.Appointments.Add(appointment);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Randevu başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Randevu oluşturulurken hata oluştu.";
            }

            // ViewBag'leri tekrar yükle
            ViewBag.Users = await _context.Users
                .Where(u => u.IsActive)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();
            
            ViewBag.Businesses = await _context.Businesses
                .Where(b => b.IsActive)
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();
            
            ViewBag.Employees = await _context.Employees
                .Include(e => e.User)
                .Where(e => e.IsActive)
                .Select(e => new { e.Id, Name = e.User.FullName })
                .ToListAsync();

            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            ViewBag.Users = await _context.Users
                .Where(u => u.IsActive)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();
            
            ViewBag.Businesses = await _context.Businesses
                .Where(b => b.IsActive)
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();
            
            ViewBag.Employees = await _context.Employees
                .Include(e => e.User)
                .Where(e => e.IsActive)
                .Select(e => new { e.Id, Name = e.User.FullName })
                .ToListAsync();

            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            try
            {
                if (id != appointment.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var existingAppointment = await _context.Appointments.FindAsync(id);
                    if (existingAppointment == null)
                    {
                        return NotFound();
                    }

                    existingAppointment.AppointmentDate = appointment.AppointmentDate;
                    existingAppointment.AppointmentTime = appointment.AppointmentTime;
                    existingAppointment.BusinessId = appointment.BusinessId;
                    existingAppointment.UserId = appointment.UserId;
                    existingAppointment.EmployeeId = appointment.EmployeeId;
                    existingAppointment.Note = appointment.Note;
                    existingAppointment.Status = appointment.Status;
                    existingAppointment.GuestFullName = appointment.GuestFullName;
                    existingAppointment.GuestEmail = appointment.GuestEmail;
                    existingAppointment.GuestPhone = appointment.GuestPhone;
                    existingAppointment.UpdatedDate = DateTime.Now;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Randevu başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Randevu güncellenirken hata oluştu.";
            }

            // ViewBag'leri tekrar yükle
            ViewBag.Users = await _context.Users
                .Where(u => u.IsActive)
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();
            
            ViewBag.Businesses = await _context.Businesses
                .Where(b => b.IsActive)
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();
            
            ViewBag.Employees = await _context.Employees
                .Include(e => e.User)
                .Where(e => e.IsActive)
                .Select(e => new { e.Id, Name = e.User.FullName })
                .ToListAsync();

            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}


