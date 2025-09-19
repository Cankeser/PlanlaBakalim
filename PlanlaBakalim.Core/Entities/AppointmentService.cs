using PlanlaBakalim.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.Core.Entities
{
    public class AppointmentService: Entity
    {
        [Display(Name = "Randevu")]
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        [Display(Name = "Hizmet")]
        public int ServiceId { get; set; }
        public BusinessService Service { get; set; } = null!;

    }
}