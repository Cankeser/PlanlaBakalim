using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanlaBakalim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Data.Configurations
{
    internal class BusinessAdressConfiguration:IEntityTypeConfiguration<BusinessAdress>
    {
        public void Configure(EntityTypeBuilder<BusinessAdress> builder)
        {
            builder.Property(ba => ba.StreetAddress)
           .IsRequired()
           .HasMaxLength(250);

            builder.HasOne(ba => ba.District)
                .WithMany(nb => nb.BusinessAdresses)
                .HasForeignKey(ba => ba.DistrictId) 
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

}
 