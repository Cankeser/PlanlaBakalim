using PlanlaBakalim.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.Core.Entities
{
    public class BusinessService : Entity
    {
        [Display(Name = "İşletme")]
        public int BusinessId { get; set; }
        [Display(Name = "Hizmet Adı")]
        public string Name { get; set; } = null!;
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
        [Display(Name = "Hizmet ücreti")]
        public decimal Price { get; set; }      
        [Display(Name = "Kategori")]
        public bool IsActive { get; set; } = true;

        public Business Business { get; set; } = null!;
        public ICollection<AppointmentService> appointmentServices { get; set; } = new HashSet<AppointmentService>();
    }

}
