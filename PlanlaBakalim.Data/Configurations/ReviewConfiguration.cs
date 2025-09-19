using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanlaBakalim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Data.Configurations
{
    
    internal class ReviewConfiguration:IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.Property(c => c.Rating).IsRequired().HasColumnType("tinyint");
            builder.Property(r => r.Comment).HasMaxLength(1000);
            builder.Property(r => r.IsVisibleOnProfile).IsRequired().HasDefaultValue(false);

        }
    }
}
