namespace PlanlaBakalim.WebUI.Models
{
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

