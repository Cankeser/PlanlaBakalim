using PlanlaBakalim.Core.DTOs;
using PlanlaBakalim.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.WebUI.Areas.BusinessPanel.Models
{
    public class RegisterBusinessVM
    {
        [Required(ErrorMessage = "İşletme adı zorunludur.")]
        [StringLength(150, ErrorMessage = "İşletme adı en fazla 150 karakter olabilir.")]
        [Display(Name = "İşletme Adı")]
        public string BusinessName { get; set; }

        [Display(Name = "İşletme E-Posta")]
        public string? BusinessEmail { get; set; }

        [Required(ErrorMessage = "İşletme telefon numarası zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "İşletme Telefon Numarası")]
        public string BusinessPhone { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
        [Display(Name = "Ad Soyad")]
        public string OwnerName { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-Posta")]
        public string OwnerEmail { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon Numarası")]
        public string OwnerPhone { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre tekrar zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        [Display(Name = "Şifre (Tekrar)")]
        public string ConfirmPassword { get; set; }

        [StringLength(500, ErrorMessage = "İşletme açıklaması en fazla 500 karakter olabilir.")]
        [Display(Name = "İşletme Açıklaması")]
        public string? BusinessDescription { get; set; }

        [Url(ErrorMessage = "Geçerli bir web sitesi adresi giriniz.")]
        [Display(Name = "İşletme Web Sitesi")]
        public string? BusinessWebsite { get; set; }

        [Required(ErrorMessage = "İşletme adresi zorunludur.")]
        [StringLength(300, ErrorMessage = "Adres en fazla 300 karakter olabilir.")]
        [Display(Name = "İşletme Adresi")]
        public string BusinessAddress { get; set; }

        [Required(ErrorMessage = "Şehir seçiniz.")]
        [Display(Name = "Şehir")]
        public int BusinessCity { get; set; }

        [Required(ErrorMessage = "İlçe seçiniz.")]
        [Display(Name = "İlçe")]
        public int BusinessDistrict { get; set; }

        [Required(ErrorMessage = "Kategori seçiniz.")]
        [Range(1, int.MaxValue, ErrorMessage = "Kategori seçimi zorunludur.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }


        [Required(ErrorMessage = "Randevu aralığı zorunludur.")]
        [Range(5, 180, ErrorMessage = "Randevu aralığı 5 ile 180 dakika arasında olmalıdır.")]
        [Display(Name = "Randevu Aralığı (Dakika)")]
        public int AppointmentSlotDuration { get; set; }

        [Display(Name = "Çalışma Saatleri")]
        public List<WorkingHoursDto> WorkingHours { get; set; } = new();
    }
}
