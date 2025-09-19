using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Models
{
    public class DashboardVM
    {
        public Business Business { get; set; }
        public int TodayAppointmentsCount { get; set; }
        public int PendingAppointmentsCount { get; set; }
        public int ActiveServicesCount { get; set; }
        public int ActiveEmployeesCount { get; set; }
        public List<Appointment> TodayAppointments { get; set; } = new();
        public List<Appointment> PendingAppointments { get; set; } = new();
        public List<ActivityItem> RecentActivities { get; set; } = new();
    }

    public class ActivityItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string ActivityType { get; set; } = string.Empty; // success, warning, info, error
    }
}

