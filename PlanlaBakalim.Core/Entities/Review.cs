using PlanlaBakalim.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Core.Entities
{
    public class Review : Entity
    {
        [Display(Name = "İşletme")]
        public int BusinessId { get; set; }        
        public Business Business { get; set; } = null!;
        [Display(Name = "Kullanıcı")]
        public int UserId { get; set; }      
        public User User { get; set; } = null!;
        [Display(Name = "Puan")]
        [Range(1, 5, ErrorMessage = "Puan 1 ile 5 arasında olmalıdır.")]
        public byte Rating { get; set; } 
        [Display(Name = "Yorum")]
        public string? Comment { get; set; }

        [Display(Name = "Profilde Gösterilsin mi?")]
        public bool IsVisibleOnProfile { get; set; } = true;

    }
}
