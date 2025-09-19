using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Models
{
    public class BusinessUpdateVM
    {
        public int Id { get; set; }

        [Display(Name ="İşletme Adı")]
        [Required(ErrorMessage = "*İşletme Adı Zorunlu")]
        public string Name { get; set; }
        [Display(Name = " E-Posta")]
        public string? Email { get; set; }
        [Display(Name = "Telefon Numarası")]
        [Required(ErrorMessage = "*Telefon Numarası Zorunlu")]
        public string Phone { get; set; }
        [Display(Name = "İşletme Açıklaması")]
        public string? Description { get; set; }
        [Display(Name = "İşletme Web Sitesi")]
        public string? Website { get; set; }
        [Display(Name = "Adres")]
        [Required(ErrorMessage = "*Adres Zorunlu")]
        public string Address { get; set; }
        [Display(Name = "Kategori")]
        [Range(1, int.MaxValue, ErrorMessage = "*Kategori Seçimi Zorunlu")]
        public int CityId { get; set; }
        [Display(Name = "İlçe")]
        [Range(1, int.MaxValue, ErrorMessage = "*İlçe Seçimi Zorunlu")]
        public int DistrictId { get; set; }
        public int AppointmentSlotDuration { get; set; }

        public string? ProfileImageUrl { get; set; }

        [Display(Name ="İşletme Profil Fotoğrafı")]
        public IFormFile? Photo { get; set; }
    }
}
