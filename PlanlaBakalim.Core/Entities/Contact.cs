using PlanlaBakalim.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Core.Entities
{
    public class Contact:Entity
    {
        [Display(Name ="Ýsim Zorunludur")]
        public string Name { get; set; }=null!;
        [Display(Name ="Email Zorunludur")]
        [EmailAddress(ErrorMessage ="Lütfen Geçerli Bir Email Adresi Giriniz")]
        public string Email { get; set; }=null!;
        [Display(Name ="Konu Zorunludur")]
        public string Subject { get; set; }=null!;
        [Display(Name ="Mesaj Zorunludur"), MaxLength(1000,ErrorMessage ="Lütfen 1000 Karakterden Kýsa Bir Mesaj Giriniz")]
        public string Message { get; set; }=null!;
        public bool IsRead { get; set; }=false;
        public DateTime? ReadAt { get; set; }
    }
}
