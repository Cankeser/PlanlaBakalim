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
    public class UserFavoritesConfiguration: IEntityTypeConfiguration<UserFavorites>
    {
        public void Configure(EntityTypeBuilder<UserFavorites> builder)
        {
            builder.HasOne(uf => uf.User)
                   .WithMany(u => u.UserFavorites)
                   .HasForeignKey(uf => uf.UserId)
                   .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(uf => uf.Business)
                   .WithMany(b => b.UserFavorites)
                   .HasForeignKey(uf => uf.BusinessId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
