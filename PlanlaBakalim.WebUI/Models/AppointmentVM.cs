using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Models
{
    public class AppointmentVM
    {
        public List<Appointment> ActiveAppointments { get; set; }= new();

        public List<Appointment> PastAppointments { get; set; }= new();
    }
}
