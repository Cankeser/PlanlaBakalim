using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.DTOs;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Service.Abstract;
using System.Threading.Tasks;

namespace PlanlaBakalim.WebUI.Controllers
{
    [Route("Randevu")]
    [AllowAnonymous]
    public class AppointmentController : BaseController
    {
        private readonly IService<BusinessService> _businessServicesService;
        private readonly IService<Employee> _employeeService;
        private readonly IBusinessService _businessService;
        private readonly IAppointmentService _appointmentService;
        private readonly IService<AppointmentService> _appointmentServicesService;

        public AppointmentController(IService<BusinessService> businessServicesService,
            IBusinessService businessService, IService<Employee> employeeService, IAppointmentService 
            appointmentService,IService<AppointmentService> appointmentServicesService)
        {
            _businessServicesService = businessServicesService;
            _businessService = businessService;
            _employeeService = employeeService;
            _appointmentService = appointmentService;
            _appointmentServicesService = appointmentServicesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int businessId)
        {
            var business = await _businessService.GetAsync(b=>b.Id==businessId&&b.IsActive);
            if (business == null) return NotFound();
            return View(business);
        }
        [HttpGet]
        [Route("Hizmetler")]
        public async Task<IActionResult> GetServices(int businessId)
        {
            if (businessId <= 0) return Json(new { success = false, message = "Geçersiz işletme ID." });

            var services = await _businessServicesService.Queryable()
                .Where(s => s.BusinessId == businessId&&s.Business.IsActive)
                .Select(s => new { s.Id, s.Name, s.Price })
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Json(new { success = true, data = services });
        }

        [HttpGet]
        [Route("Calisanlar")]
        public async Task<IActionResult> GetEmployees(int businessId)
        {
            var employees = await _employeeService.Queryable()
                .Where(e => e.BusinessId == businessId && e.IsActive)
                .Include(e => e.User)
                .Select(e => new
                {
                    e.Id,
                    e.Position,
                    User = new { FullName = e.User.FullName }
                })
                .ToListAsync();
            if (employees == null || !employees.Any())
                return Json(new { success = false, message = "Çalışan bulunamadı." });
            return Json(new { success = true, data = employees });
        }

        [HttpGet]
        [Route("UygunSaatler")]
        public async Task<IActionResult> GetAvailableSlots(int businessId, int employeeId, string date)
        {
            if (!DateTime.TryParse(date, out var appointmentDate))
                return Json(new { success = false, message = "Geçersiz tarih." });

            var business = await _businessService.GetAsync(b => b.Id == businessId && b.IsActive);
            if (business == null) return Json(new { success = false, message = "İşletme bulunamadı." });

            var slots = await _appointmentService.GetTimeSlotsAsync(businessId, appointmentDate, employeeId, business.AppointmentSlotDuration);

            return Json(new { success = true, data = slots });
        }

        [HttpPost]
        [Route("Olustur")]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Form geçersiz" });

            int? userId = null;

            if (dto.UserType == "account")
            {
                // Kullanıcı giriş yaptıysa ID’yi al (Id veya NameIdentifier)
                if (User.Identity.IsAuthenticated)
                {
                    var idClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    userId = int.TryParse(idClaim, out var parsed) ? parsed : null;
                }
                if (userId == null)
                    return Json(new { success = false, message = "Kullanıcı giriş yapmamış" });
            }
            else
            {
                // Misafir kullanıcı doğrulaması
                if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Email))
                    return Json(new { success = false, message = "Misafir bilgileri eksik" });
            }

            var business= await _businessService.GetAsync(b => b.Id == dto.BusinessId && b.IsActive);
            if (business == null)
                return Json(new { success = false, message = "İşletme bulunamadı." });

            var appointment = new Appointment
            {
                BusinessId = business.Id,
                EmployeeId = dto.EmployeeId,
                AppointmentDate = DateTime.Parse(dto.AppointmentDate),
                AppointmentTime = TimeSpan.Parse(dto.AppointmentTime),
                UserId = userId, 
                GuestFullName = dto.UserType == "guest" ? dto.FullName : null,
                GuestEmail = dto.UserType == "guest" ? dto.Email : null,
                GuestPhone = dto.UserType == "guest" ? dto.Phone : null,
                Note = dto.Note ?? null,
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
            int sonuc = await _appointmentServicesService.SaveChangesAsync();

            return Json(new { success = true, message = "Randevu oluşturuldu" });
        }

    }
}
