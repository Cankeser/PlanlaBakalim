using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;
using PlanlaBakalim.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Data.Configurations
{
    internal class UserConfiguration:IEntityTypeConfiguration<User>
    {

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
            builder.Property(u => u.Phone).IsRequired().HasMaxLength(15);
            builder.Property(u => u.PasswordHash).HasMaxLength(250);
            builder.Property(u => u.Role).IsRequired();
            builder.Property(u => u.IsActive).IsRequired().HasDefaultValue(true);
            builder.HasMany(u => u.Employees)
                            .WithOne(e => e.User)
                            .HasForeignKey(e => e.UserId)
                            .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Appointments)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasData(
               new User
               {
                   Id = 1,
                   FullName = "Test User",
                   Email = "admin@planlabakalim.com",
                   Phone = "5551234567",
                   PasswordHash = PasswordHasher.Hash("Admin123!"),
                   Role = UserRole.Admin,
                   IsActive = true
               }
           );

        }
    }
}
