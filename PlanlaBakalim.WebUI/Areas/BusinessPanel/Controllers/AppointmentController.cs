using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.WebUI.Models;
using PlanlaBakalim.WebUI.Areas.BusinessPanel.Models;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Controllers
{
    [Area("BusinessPanel")]
    [Authorize(AuthenticationSchemes = "BusinessScheme", Policy = "BusinessPolicy")]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IBusinessService _businessService;
        private readonly IService<Employee> _employeeService;
        private readonly IService<Core.Entities.BusinessService> _businessServicesService;
        private readonly IService<AppointmentService> _appointmentServicesService;
 


        public AppointmentController(IAppointmentService appointmentService, IService<Employee> employeeService, IBusinessService businessService, IService<Core.Entities.BusinessService> businessServicesService, IService<AppointmentService> appointmentServicesService)
        {
            _appointmentService = appointmentService;
            _employeeService = employeeService;
            _businessService = businessService;
            _businessServicesService = businessServicesService;
            _appointmentServicesService = appointmentServicesService;
        }

        public async Task<IActionResult> Index()
        {
           var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
         

            var appointments = await _appointmentService.GetAppointmentsByBusinessAsync(businessId); 


            var now = DateTime.Now;
            var model = new AppointmentVM
            {
                // Aktif randevular: Onaylandi/Beklemede durumunda VE gelecekte olanlar
                ActiveAppointments = appointments.Where(a => 
                    (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Pending) && 
                    a.AppointmentDate.Date.Add(a.AppointmentTime) >= now).ToList(),
                
                // Geçmiş randevular: 
                // 1. İptalEdildi veya Tamamlandi durumundaki tüm randevular
                // 2. Zamanı geçmiş Onaylandi/Beklemede randevular
                PastAppointments = appointments.Where(a => 
                    a.Status == AppointmentStatus.Cancelled || 
                    a.Status == AppointmentStatus.Completed || 
                    (a.AppointmentDate.Date.Add(a.AppointmentTime) < now && 
                     (a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Pending))).ToList(),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointmentDetails(int id)
        {
            var appointment = await _appointmentService.GetAppointmentDetailsAsync(id);


            if (appointment == null)
                return NotFound();

            return Json(new
            {
                Name = appointment?.User == null ? appointment?.GuestFullName : appointment.User.FullName,
                Phone = appointment?.User == null ? appointment?.GuestPhone : appointment.User.Phone,
                Email = appointment?.User == null ? appointment?.GuestEmail : appointment.User.Email,
                Avatar = appointment?.User?.ProfileUrl ?? "/img/userProfile.jpg",
                Employee = appointment?.Employee.User.FullName,
                Date = $"{appointment?.AppointmentDate.ToString("dd MMM yyyy")} - {appointment?.AppointmentTime.ToString(@"hh\:mm")}",
                Status = appointment?.Status.ToString(),
                Notes = appointment?.Note,
                Services = appointment?.AppointmentServices.Select(asv => asv.Service.Name).ToList()
            });

        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
            var employees = await _employeeService.Queryable()
               .Where(e => e.BusinessId == businessId && e.IsActive)
               .Include(e => e.User)
               .Select(e => new
               {
                   e.Id,
                   e.Position,
                   User = new { e.User.FullName, Avatar = e.User.ProfileUrl ?? "/img/userProfile.jpg" }
               })
              .ToListAsync();
            if (employees == null || employees.Count == 0)
                return Json(new { success = false, message = "Aktif çalışan bulunamadı." });

            return Json(new { success = true, data = employees });
        }
        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
            if (businessId <= 0) return Json(new { success = false, message = "Geçersiz işletme ID." });

            var services = await _businessServicesService.Queryable()
                .Where(s => s.BusinessId == businessId)
                .Select(s => new { s.Id, s.Name, s.Price })
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Json(new { success = true, data = services });
        }
        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int employeeId, string date)
        {
            var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
            if (!DateTime.TryParse(date, out var appointmentDate))
                return Json(new { success = false, message = "Geçersiz tarih." });

            var business = await _businessService.FindAsync(businessId);// İşletme ID'si test amaçlı sabit
            if (business == null) return Json(new { success = false, message = "İşletme bulunamadı." });

            var slots = await _appointmentService.GetTimeSlotsAsync(businessId, appointmentDate, employeeId, business.AppointmentSlotDuration);// İşletme ID'si test amaçlı sabit


            return Json(new { success = true, data = slots });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Form geçersiz" });

            var businessId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "BusinessId")?.Value);
            int? userId = null;
             
                // Misafir kullanıcı doğrulaması
                if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Email))
                    return Json(new { success = false, message = "Misafir bilgileri eksik" });
          var business = await _businessService.GetAsync(b => b.Id == businessId && b.IsActive);    
            if (business == null)
                return Json(new { success = false, message = "İşletme bulunamadı." });
            var appointment = new Appointment
            {
                BusinessId = business.Id,
                EmployeeId = dto.EmployeeId,
                AppointmentDate = DateTime.Parse(dto.AppointmentDate),
                AppointmentTime = TimeSpan.Parse(dto.AppointmentTime),
                UserId = null,
                GuestFullName = dto.UserType == "guest" ? dto.FullName : null,
                GuestEmail = dto.UserType == "guest" ? dto.Email : null,
                GuestPhone = dto.UserType == "guest" ? dto.Phone : null,
                Note = dto.Note ?? null
            };

            var created = await _appointmentService.CreateAppointmentAsync(appointment);
            if (!created)
                return Json(new { success = false, message = "Seçilen zaman dilimi dolu, lütfen başka bir zaman seçin." });

            foreach (var serviceId in dto.SelectedServices)
            {
                _appointmentServicesService.Add(new AppointmentService
                {
                    ServiceId = serviceId,
                    AppointmentId = appointment.Id,
                });
            }
            await _appointmentServicesService.SaveChangesAsync();

            return Json(new { success = true, message = "Randevu oluşturuldu" });
        }


        [HttpPost]
        public async Task<IActionResult> Cancel(int appointmentId)
        {
            var appointment = await _appointmentService.GetAsync(a => a.Id == appointmentId);
            if (appointment == null)
            {
                TempData["Notification"] = "Randevu bulunamadı!";
                TempData["NotificationType"] = "error";
                return RedirectToAction("Index", "Appointment", new {area="BusinessPanel"});
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedDate = DateTime.Now;
            _appointmentService.Update(appointment);
            await _appointmentService.SaveChangesAsync();

            TempData["Notification"] = "Randevu başarıyla iptal edildi.";
            TempData["NotificationType"] = "success";
            return RedirectToAction("Index", "Appointment", new { area = "BusinessPanel" });
        }
        [HttpPost]
        public async Task<IActionResult> Confirm(int appointmentId)
        {
            var appointment = await _appointmentService.GetAsync(a => a.Id == appointmentId);
            if (appointment == null)
            {
                TempData["Notification"] = "Randevu bulunamadı!";
                TempData["NotificationType"] = "error";
                return RedirectToAction("Index", "Appointment", new { area = "BusinessPanel" });
            }

            appointment.Status = AppointmentStatus.Confirmed;
            appointment.UpdatedDate = DateTime.Now;
            _appointmentService.Update(appointment);
            await _appointmentService.SaveChangesAsync();

            TempData["Notification"] = "Randevu Onaylandı.";
            TempData["NotificationType"] = "success";
            return RedirectToAction("Index", "Appointment", new { area = "BusinessPanel" });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptAppointment([FromBody] AcceptRejectRequest request)
        {
            try
            {
                var appointment = await _appointmentService.FindAsync(request.AppointmentId);
                if (appointment == null)
                {
                    return Json(new { success = false, message = "Randevu bulunamadı." });
                }

                appointment.Status = AppointmentStatus.Confirmed;
                appointment.UpdatedDate = DateTime.Now;
                
                _appointmentService.Update(appointment);
                await _appointmentService.SaveChangesAsync();

                return Json(new { success = true, message = "Randevu başarıyla onaylandı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Randevu onaylanırken bir hata oluştu." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectAppointment([FromBody] AcceptRejectRequest request)
        {
            try
            {
                var appointment = await _appointmentService.FindAsync(request.AppointmentId);
                if (appointment == null)
                {
                    return Json(new { success = false, message = "Randevu bulunamadı." });
                }

                appointment.Status = AppointmentStatus.Cancelled;
                appointment.UpdatedDate = DateTime.Now;
                
                _appointmentService.Update(appointment);
                await _appointmentService.SaveChangesAsync();

                return Json(new { success = true, message = "Randevu başarıyla reddedildi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Randevu reddedilirken bir hata oluştu." });
            }
        }

    }
}
