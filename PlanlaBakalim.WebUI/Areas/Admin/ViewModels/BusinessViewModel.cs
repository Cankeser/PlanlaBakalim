using System.ComponentModel.DataAnnotations;
using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.WebUI.Areas.Admin.ViewModels
{
    public class BusinessCreateViewModel
    {
        [Required(ErrorMessage = "İşletme adı gereklidir")]
        [Display(Name = "İşletme Adı")]
        [StringLength(100, ErrorMessage = "İşletme adı en fazla 100 karakter olabilir")]
        public string Name { get; set; } = null!;

        [Display(Name = "İşletme Profil Fotoğrafı")]
        public string? ProfileImageUrl { get; set; }

        [Required(ErrorMessage = "Telefon numarası gereklidir")]
        [Display(Name = "Telefon Numarası")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string PhoneNumber { get; set; } = null!;

        [Display(Name = "E-Posta")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string? Email { get; set; }

        [Display(Name = "Web Sitesi")]
        [Url(ErrorMessage = "Geçerli bir web sitesi URL'si giriniz")]
        public string? Website { get; set; }

        [Display(Name = "Açıklama")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Kategori seçimi gereklidir")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Randevu Aralığı (Dakika)")]
        [Range(15, 240, ErrorMessage = "Randevu aralığı 15-240 dakika arasında olmalıdır")]
        public int AppointmentSlotDuration { get; set; } = 30;

        [Required(ErrorMessage = "İşletme sahibi seçimi gereklidir")]
        [Display(Name = "İşletme Sahibi")]
        public int OwnerId { get; set; }

        // Dropdown listeler için
        public List<Category>? Categories { get; set; }
        public List<User>? Users { get; set; }
    }

    public class BusinessEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İşletme adı gereklidir")]
        [Display(Name = "İşletme Adı")]
        [StringLength(100, ErrorMessage = "İşletme adı en fazla 100 karakter olabilir")]
        public string Name { get; set; } = null!;

        [Display(Name = "İşletme Profil Fotoğrafı")]
        public string? ProfileImageUrl { get; set; }

        [Required(ErrorMessage = "Telefon numarası gereklidir")]
        [Display(Name = "Telefon Numarası")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string PhoneNumber { get; set; } = null!;

        [Display(Name = "E-Posta")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string? Email { get; set; }

        [Display(Name = "Web Sitesi")]
        [Url(ErrorMessage = "Geçerli bir web sitesi URL'si giriniz")]
        public string? Website { get; set; }

        [Display(Name = "Açıklama")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Kategori seçimi gereklidir")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Randevu Aralığı (Dakika)")]
        [Range(15, 240, ErrorMessage = "Randevu aralığı 15-240 dakika arasında olmalıdır")]
        public int AppointmentSlotDuration { get; set; } = 30;

        [Required(ErrorMessage = "İşletme sahibi seçimi gereklidir")]
        [Display(Name = "İşletme Sahibi")]
        public int OwnerId { get; set; }

        // Dropdown listeler için
        public List<Category>? Categories { get; set; }
        public List<User>? Users { get; set; }
    }
}
