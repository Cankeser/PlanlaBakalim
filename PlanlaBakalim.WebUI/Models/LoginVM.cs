using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.WebUI.Models
{
    public class LoginVM
    {
        [Display(Name = "Mail adresi")]
        [Required(ErrorMessage = "Mail Adresi Boş Geçilemez!")]
        public string Email { get; set; }=null!;
        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre Boş Geçilemez!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }=null!;

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }

    }
}
