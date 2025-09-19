using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.WebUI.Models;


namespace PlanlaBakalim.WebUI.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly IService<User> _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly IService<UserFavorites> _userFavorites;
        private readonly IService<Appointment> _appointmentEntityService;

        public ProfileController(IService<User> userService, IAppointmentService appointmentService, IService<UserFavorites> userFavorites, IService<Appointment> appointmentEntityService)
        {
            _userService = userService;
            _appointmentService = appointmentService;
            _userFavorites = userFavorites;
            _appointmentEntityService = appointmentEntityService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
               var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

                var user = await _userService.GetAsync(u => u.Id == userId && u.IsActive);
                if (user == null)
                {
                    return NotFound("Kullanıcı bulunamadı");
                }

     
                var allAppointments = await _appointmentService.Queryable()
                    .Include(a => a.Business)
                    .Include(a => a.Business.BusinessAddress)
                    .Include(a => a.Business.BusinessAddress.District)
                    .Include(a => a.Business.BusinessAddress.District.City)
                    .Where(a => a.UserId == userId)
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentTime)
                    .ToListAsync();

  
                var userFavorites = await _userFavorites.Queryable()
                    .Include(uf => uf.Business)
                    .Where(uf => uf.UserId == userId&&uf.Business.IsActive)
                    .ToListAsync();

                var now = DateTime.Now;
                var nextAppointment = allAppointments
                    .Where(a => a.AppointmentDate.Date > now.Date ||
                               (a.AppointmentDate.Date == now.Date && a.AppointmentTime > now.TimeOfDay))
                    .FirstOrDefault();

                // Aktiviteleri oluştur
                var activities = new List<UserActivityVM>();
                foreach (var a in allAppointments)
                {
                    // Oluşturuldu
                    activities.Add(new UserActivityVM
                    {
                        Type = "AppointmentCreated",
                        Title = "Randevu oluşturuldu",
                        Description = $"{a.Business?.Name} • {a.AppointmentTime:hh\\:mm}, {a.AppointmentDate:dd MMM yyyy}",
                        CreatedAt = a.CreatedDate,
                        Icon = "calendar-plus",
                        Link = $"/Profile/Appointments/{a.Id}",
                        AppointmentId = a.Id
                    });

                    if (a.Status == AppointmentStatus.Confirmed)
                    {
                        activities.Add(new UserActivityVM
                        {
                            Type = "AppointmentApproved",
                            Title = "Randevunuz onaylandı",
                            Description = $"{a.Business?.Name} • {a.AppointmentTime:hh\\:mm}, {a.AppointmentDate:dd MMM yyyy}",
                            CreatedAt = a.UpdatedDate ?? a.CreatedDate,
                            Icon = "check",
                            Link = $"/Profile/Appointments/{a.Id}",
                            AppointmentId = a.Id
                        });
                    }
                    else if (a.Status == AppointmentStatus.Cancelled)
                    {
                        activities.Add(new UserActivityVM
                        {
                            Type = "AppointmentCancelled",
                            Title = "Randevunuz iptal edildi",
                            Description = $"{a.Business?.Name} • {a.AppointmentTime:hh\\:mm}, {a.AppointmentDate:dd MMM yyyy}",
                            CreatedAt = a.UpdatedDate ?? a.CreatedDate,
                            Icon = "times",
                            Link = $"/Profile/Appointments/{a.Id}",
                            AppointmentId = a.Id
                        });
                    }
                    else if (a.Status == AppointmentStatus.Completed)
                    {
                        activities.Add(new UserActivityVM
                        {
                            Type = "AppointmentCompleted",
                            Title = "Randevu tamamlandı",
                            Description = $"{a.Business?.Name} • {a.AppointmentTime:hh\\:mm}, {a.AppointmentDate:dd MMM yyyy}",
                            CreatedAt = a.UpdatedDate ?? a.CreatedDate,
                            Icon = "star",
                            Link = $"/Profile/Appointments/{a.Id}",
                            AppointmentId = a.Id
                        });
                    }
                }

    
                activities = activities
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(6)
                    .ToList();

                var model = new ProfileVM
                {
                    User = user,
                    NextAppointment = nextAppointment,
                    Favorites = userFavorites ?? new List<UserFavorites>(),
                    Appointments = new AppointmentVM
                    {
                        ActiveAppointments = allAppointments?
                            .Where(a => a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Pending)
                            .ToList() ?? new List<Appointment>(),
                        PastAppointments = allAppointments?
                            .Where(a => a.Status == AppointmentStatus.Cancelled || a.Status == AppointmentStatus.Completed)
                            .ToList() ?? new List<Appointment>()
                    },
                    Activities = activities
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Profile/Appointments/{id}")]
        public async Task<IActionResult> GetAppointmentDetail(int id)
        {
            try
            {
                var currentUserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
                var appointment = await _appointmentEntityService.Queryable()
                    .Include(a => a.Business)
                    .Include(a => a.Employee)
                    .Include(a => a.AppointmentServices)
                        .ThenInclude(s => s.Service)
                    .FirstOrDefaultAsync(a => a.Id == id && a.UserId == currentUserId);

                if (appointment == null) return NotFound();

                var dto = new
                {
                    Id = appointment.Id,
                    Business = new { appointment.Business.Id, appointment.Business.Name, appointment.Business.ProfileImageUrl },
                    Employee = new { appointment.Employee.Id, Name = appointment.Employee.User != null ? appointment.Employee.User.FullName : string.Empty },
                    Date = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                    Time = appointment.AppointmentTime.ToString(@"hh\:mm"),
                    Status = appointment.Status.ToString(),
                    Note = appointment.Note,
                    Services = appointment.AppointmentServices.Select(s => new { s.ServiceId, s.Service.Name, s.Service.Price }).ToList(),
                    Total = appointment.AppointmentServices.Sum(s => (decimal?)s.Service.Price) ?? 0m
                };

                return Json(new { success = true, data = dto });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}