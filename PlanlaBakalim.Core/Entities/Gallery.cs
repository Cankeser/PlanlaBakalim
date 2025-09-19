using PlanlaBakalim.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.Core.Entities
{
    public class Gallery : Entity
    {
        [Display(Name = "Resim URL")]
        public string ImageUrl { get; set; } = string.Empty;
        
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;
        
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "İşletme")]
        public Business Business { get; set; } = null!;
        public int BusinessId { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Profilde görünsün mü?")]
        public bool IsVisibleProfile { get; set; } = true;
    }
}
