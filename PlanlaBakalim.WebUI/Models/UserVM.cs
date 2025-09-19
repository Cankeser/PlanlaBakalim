using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.WebUI.Models
{
    public class UserVM
    {
        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "*Ad Soyad Gerekli")]
        public string FullName { get; set; }
        [Display(Name = "Mail adresi")]
        [Required(ErrorMessage = "*Mail Adresi Gerekli")]
        public string Email { get; set; }
        [Display(Name = "Telefon numarası")]
        [Required(ErrorMessage = "*Telefon Numarası Gerekli")]
        public string Phone { get; set; } = null!;

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre Gerekli")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Şifre (Tekrar)")]
        [Required(ErrorMessage = "*Şifre Tekrar Gerekli")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "*Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }
}
