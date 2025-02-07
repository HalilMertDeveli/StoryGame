using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MurderGame.Entities.Domains;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MurderGame.Entities.Domains.MurderGame.Entities.Domains;

namespace MurderGame.DataAccess.Configurations
{
    public class ApplicationUserTableConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(u => u.Id); // IdentityUser<int> zaten Id içeriyor

            builder.Property(u => u.UserNickName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(u => u.UserNickName).IsUnique(); // Nickname benzersiz olmalı

            builder.Property(u => u.ProfilePicture)
                .HasMaxLength(255);

            builder.Property(u => u.Bio)
                .HasMaxLength(500);

            builder.Property(u => u.IsVerified)
                .IsRequired();

            builder.Property(u => u.IsBanned)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.LastLoginDate)
                .IsRequired(false);

            builder.HasOne(u => u.ApplicationUserProfileDetails)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<ApplicationUserProfileDetails>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
