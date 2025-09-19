using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanlaBakalim.Core.Entities;
using PlanlaBakalim.Core.Enums;

namespace PlanlaBakalim.Data.Configurations
{
    internal class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.Property(a => a.GuestFullName).HasMaxLength(150);
            builder.Property(a => a.GuestEmail).HasMaxLength(150);
            builder.Property(a => a.GuestPhone).HasMaxLength(15);
            builder.Property(a => a.Status).IsRequired().HasDefaultValue(AppointmentStatus.Pending);
            builder.HasMany(a => a.AppointmentServices).WithOne(As => As.Appointment).HasForeignKey(As => As.AppointmentId).OnDelete(DeleteBehavior.Restrict);


        }
    }
}
