using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;
namespace PlanlaBakalim.WebUI.Models
{
    public class ProfileVM
    {
        public Appointment NextAppointment { get; set; }
        public User User { get; set; }
        public AppointmentVM Appointments { get; set; } = new();
        public List<UserFavorites> Favorites { get; set; } = new();
        public List<UserActivityVM> Activities { get; set; } = new();

    }

    public class UserActivityVM
    {
        public string Type { get; set; } = string.Empty; // AppointmentCreated, AppointmentApproved, ...
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Icon { get; set; } = "info";
        public string? Link { get; set; }
        public int? AppointmentId { get; set; }
    }
}
