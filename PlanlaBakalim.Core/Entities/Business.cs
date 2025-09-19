using PlanlaBakalim.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.Core.Entities
{
    public class Business : Entity
    {
        [Display(Name = "İşletme Adı")]
        public string Name { get; set; } = null!;

        [Display(Name = "İşletme Profil Fotoğrafı")]
        public string? ProfileImageUrl { get; set; }

        [Display(Name = "Telefon Numarası")]
        public string PhoneNumber { get; set; } = null!;

        [Display(Name = "E-Posta")]
        public string? Email { get; set; }

        [Display(Name = "Web Sitesi")]
        public string? Website { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Randevu Aralığı (Dakika)")]
        public int AppointmentSlotDuration { get; set; } = 30;
          

        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "İşletme Sahibi")]
        public int OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        public BusinessAdress? BusinessAddress { get; set; }
        public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
        public ICollection<BusinessService> Services { get; set; } = new HashSet<BusinessService>();
        public ICollection<BusinessWorkingHour> WorkingHours { get; set; } = new HashSet<BusinessWorkingHour>();
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public ICollection<Gallery> Galleries { get; set; } = new HashSet<Gallery>();
        public ICollection<UserFavorites> UserFavorites { get; set; } = new HashSet<UserFavorites>();
    }
}