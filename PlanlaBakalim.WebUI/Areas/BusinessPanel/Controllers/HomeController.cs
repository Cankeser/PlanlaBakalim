using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.WebUI.Areas.BusinessPanel.Models;
using System.Security.Claims;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Controllers
{
    [Area("BusinessPanel")]
    [Authorize(AuthenticationSchemes = "BusinessScheme", Policy = "BusinessPolicy")]
    public class HomeController : Controller
    {
        private readonly IBusinessService _businessService;
        private readonly IAppointmentService _appointmentService;

        public HomeController(IBusinessService businessService, IAppointmentService appointmentService)
        {
            _businessService = businessService;
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var business = await _businessService.GetBusinessByOwnerIdAsync(userId);

            if (business == null)
            {
                return RedirectToAction("Logout", "Account", new { area = "BusinessPanel" });
            }

            // Dashboard verilerini service'lerden al
            var todayAppointments = await _appointmentService.GetTodayAppointmentsAsync(business.Id);
            var pendingAppointments = await _appointmentService.GetPendingAppointmentsAsync(business.Id);
            var todayAppointmentsCount = await _appointmentService.GetTodayAppointmentsCountAsync(business.Id);
            var pendingAppointmentsCount = pendingAppointments.Count;
            var activeServicesCount = await _businessService.GetActiveServicesCountAsync(business.Id);
            var activeEmployeesCount = await _businessService.GetActiveEmployeesCountAsync(business.Id);
            var recentAppointments = await _appointmentService.GetRecentAppointmentsAsync(business.Id);
            var recentServices = await _businessService.GetRecentServicesAsync(business.Id);
            var recentEmployees = await _businessService.GetRecentEmployeesAsync(business.Id);

            // Son aktiviteler
            var recentActivities = new List<ActivityItem>();

            // Son randevular
            foreach (var appointment in recentAppointments)
            {
                var userName = appointment.User?.FullName ?? 
                              appointment.GuestFullName ?? "Misafir";
                
                switch (appointment.Status)
                {
                    case AppointmentStatus.Confirmed:
                        recentActivities.Add(new ActivityItem
                        {
                            Title = "Randevu Onaylandı",
                            Description = $"{userName}'ın randevusu onaylandı",
                            CreatedAt = appointment.UpdatedDate ?? appointment.CreatedDate,
                            ActivityType = "success"
                        });
                        break;
                    case AppointmentStatus.Cancelled:
                        recentActivities.Add(new ActivityItem
                        {
                            Title = "Randevu İptal Edildi",
                            Description = $"{userName}'ın randevusu iptal edildi",
                            CreatedAt = appointment.UpdatedDate ?? appointment.CreatedDate,
                            ActivityType = "warning"
                        });
                        break;
                    case AppointmentStatus.Completed:
                        recentActivities.Add(new ActivityItem
                        {
                            Title = "Randevu Tamamlandı",
                            Description = $"{userName}'ın randevusu tamamlandı",
                            CreatedAt = appointment.UpdatedDate ?? appointment.CreatedDate,
                            ActivityType = "success"
                        });
                        break;
                }
            }

            // Son eklenen hizmetler
            foreach (var service in recentServices)
            {
                recentActivities.Add(new ActivityItem
                {
                    Title = "Yeni Hizmet Eklendi",
                    Description = $"{service.Name} hizmeti eklendi",
                    CreatedAt = service.CreatedDate,
                    ActivityType = "info"
                });
            }

            // Son eklenen çalışanlar
            foreach (var employee in recentEmployees)
            {
                var employeeName = employee.User?.FullName ?? "Çalışan";
                recentActivities.Add(new ActivityItem
                {
                    Title = "Yeni Çalışan Eklendi",
                    Description = $"{employeeName} çalışan olarak eklendi",
                    CreatedAt = employee.CreatedDate,
                    ActivityType = "info"
                });
            }

            // Aktiviteleri tarihe göre sırala
            recentActivities = recentActivities
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToList();

            var dashboardVM = new DashboardVM
            {
                Business = business,
                TodayAppointmentsCount = todayAppointmentsCount,
                PendingAppointmentsCount = pendingAppointmentsCount,
                ActiveServicesCount = activeServicesCount,
                ActiveEmployeesCount = activeEmployeesCount,
                TodayAppointments = todayAppointments,
                PendingAppointments = pendingAppointments,
                RecentActivities = recentActivities
            };

            return View(dashboardVM);
        }
    }
}
