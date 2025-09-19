using PlanlaBakalim.Core.Base;
using PlanlaBakalim.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.Core.Entities
{
    public class User : Entity
    {
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = null!;
        [Display(Name = "Mail adresi")]
        public string Email { get; set; } = null!;
        [Display(Name = "Kullanıcı Profil Fotoğrafı")]
        public string? ProfileUrl { get; set; } 
        [Display(Name = "Telefon numarası")]
        public string Phone { get; set; } = null!;
        public string? PasswordHash { get; set; }
        public UserRole Role { get; set; } = UserRole.Musteri;

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;
        [Display(Name = "Güncelleme Tarihi")]
        public DateTime? UpdatedDate { get; set; }

        public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        public ICollection<UserFavorites> UserFavorites { get; set; } = new HashSet<UserFavorites>();
    }
}
