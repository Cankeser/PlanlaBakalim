using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanlaBakalim.Core.Entities;

namespace PlanlaBakalim.Data.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(250);
            builder.Property(c => c.OrderNo).IsRequired().HasDefaultValue(1);
            builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(c => c.ParentId).HasDefaultValue(null);

            builder.HasMany(c => c.SubCategories)
                   .WithOne(c => c.ParentCategory)
                   .HasForeignKey(c => c.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(c => c.Businesses)
                   .WithOne(b => b.Category)
                   .HasForeignKey(b => b.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Seed data
            builder.HasData(
               new Category { 
                   Id = 1,
                   Name = "Erkek Kuaför",
                   Description = "Saç kesimi ve bakýmý", 
                   OrderNo = 1,
                   IsActive = true,
                   ParentId = null },
               new Category {
                   Id = 2,
                   Name = "Kadýn Kuaför", 
                   Description = "Saç kesimi ve bakýmý", 
                   OrderNo = 2, 
                   IsActive = true, 
                   ParentId = null }
             );

        }
    }
}
