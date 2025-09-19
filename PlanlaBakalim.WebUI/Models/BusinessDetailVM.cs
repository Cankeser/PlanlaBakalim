using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Models
{
    public class BusinessDetailVM
    {
        public string? PhoneNumber { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public string? NearestAppointmentTime { get; set; }
        public BusinessVM Business { get; set; } = new();
        public List<BusinessWorkingHour> BusinessWorkingHours { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public List<BusinessService> Services { get; set; } = new();
        public List<Employee> Employees { get; set; } = new();
        public List<Gallery> Galleries { get; set; } = new();

    }
}
