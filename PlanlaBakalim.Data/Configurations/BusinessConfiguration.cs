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
    internal class BusinessConfiguration:IEntityTypeConfiguration<Business>
    {
        public void Configure(EntityTypeBuilder<Business> builder)
        {
            builder.Property(b => b.Name).IsRequired().HasMaxLength(150);
            builder.Property(b => b.Email).HasMaxLength(150);
            builder.Property(b => b.Website).HasMaxLength(200);
            builder.Property(b => b.Description).HasMaxLength(1000);
            builder.Property(b => b.IsActive).IsRequired().HasDefaultValue(true);
            builder.HasMany(b => b.Employees)
                   .WithOne(e => e.Business)
                   .HasForeignKey(e => e.BusinessId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(b => b.Reviews)
                   .WithOne(r => r.Business)
                   .HasForeignKey(r => r.BusinessId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(b => b.Services)
                   .WithOne(a => a.Business)
                   .HasForeignKey(a => a.BusinessId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(b => b.WorkingHours)
                     .WithOne(w => w.Business)
                     .HasForeignKey(w => w.BusinessId)
                     .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(b => b.BusinessAddress)
                     .WithOne(ba => ba.Business)
                     .HasForeignKey<BusinessAdress>(ba => ba.BusinessId)
                     .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
