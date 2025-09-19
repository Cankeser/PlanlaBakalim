using PlanlaBakalim.Core.Base;
using PlanlaBakalim.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Core.Entities
{
    public class Employee: Entity
    {
        [Display(Name = "Kullanıcı Id")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Display(Name = "İşletme Id")]
        public int BusinessId { get; set; }
        public Business Business { get; set; } = null!;

        [Display(Name = "Çalışan Pozisyonu")]
        public string Position { get; set; }= null!;

        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "İşe Başlama Tarihi")]
        public DateTime StartDate { get; set; } 

        [Display(Name = "İşten Ayrılma Tarihi")]
        public DateTime? EndDate { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
    }
}
