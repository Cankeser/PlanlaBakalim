using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Models
{
    public class EmployeeVM
    {
        public int Id { get; set; }

        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = null!;
        [Display(Name = "Mail adresi")]
        public string Email { get; set; } = null!;
        [Display(Name = "Kullanıcı Profil Fotoğrafı")]
        public string? ProfileUrl { get; set; }
        [Display(Name = "Telefon numarası")]
        public string Phone { get; set; } = null!;

        [Display(Name = "Kullanıcı Id")]
        public int UserId { get; set; }

        [Display(Name = "İşletme Id")]
        public int BusinessId { get; set; }

        [Display(Name = "Çalışan Pozisyonu")]
        public string Position { get; set; } = null!;

        [Display(Name = "İşe Başlama Tarihi")]
        public DateTime StartDate { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
