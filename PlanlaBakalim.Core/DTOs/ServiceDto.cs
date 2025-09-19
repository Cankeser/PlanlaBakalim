using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Core.DTOs
{
    public class ServiceDto
    {
        [Display(Name = "��letme")]
        public int BusinessId { get; set; }
        [Display(Name = "Hizmet Ad�")]
        public string Name { get; set; } = null!;
        [Display(Name = "A��klama")]
        public string? Description { get; set; }
        [Display(Name = "Hizmet �creti")]
        public decimal Price { get; set; }
        [Display(Name = "Kategori")]
        public bool IsActive { get; set; } = true;
    }
}
