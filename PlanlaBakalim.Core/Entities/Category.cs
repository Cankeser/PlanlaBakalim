using PlanlaBakalim.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.Core.Entities
{
    public class Category : Entity
    {
        [Display(Name = "Kategori Adý")]
        public string Name { get; set; } = null!;

        [Display(Name = "Açýklama")]
        public string? Description { get; set; }

        [Display(Name = "Üst Kategori")]
        public int? ParentId { get; set; }
        public Category? ParentCategory { get; set; }

        [Display(Name = "Sýra No")]
        public int OrderNo { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }

        // Navigations
        public ICollection<Business> Businesses { get; set; } = new HashSet<Business>();
        public ICollection<Category> SubCategories { get; set; } = new HashSet<Category>();
    }
}
