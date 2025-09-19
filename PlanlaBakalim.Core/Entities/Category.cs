using PlanlaBakalim.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace PlanlaBakalim.Core.Entities
{
    public class Category : Entity
    {
        [Display(Name = "Kategori Ad�")]
        public string Name { get; set; } = null!;

        [Display(Name = "A��klama")]
        public string? Description { get; set; }

        [Display(Name = "�st Kategori")]
        public int? ParentId { get; set; }
        public Category? ParentCategory { get; set; }

        [Display(Name = "S�ra No")]
        public int OrderNo { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "G�ncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }

        // Navigations
        public ICollection<Business> Businesses { get; set; } = new HashSet<Business>();
        public ICollection<Category> SubCategories { get; set; } = new HashSet<Category>();
    }
}
