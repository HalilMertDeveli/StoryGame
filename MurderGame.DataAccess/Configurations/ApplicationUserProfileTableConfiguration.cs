using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MurderGame.Entities.Domains;
using MurderGame.Entities.Domains.MurderGame.Entities.Domains;

namespace MurderGame.DataAccess.Configurations
{
    public class ApplicationUserProfileTableConfiguration : IEntityTypeConfiguration<ApplicationUserProfileDetails>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserProfileDetails> builder)
        {
            builder.HasKey(p => p.ProfileDetailsId);

            builder.Property(p => p.DisplayName)
                .HasMaxLength(50);

            builder.Property(p => p.ProfilePicture)
                .HasMaxLength(255);

            builder.Property(p => p.Bio)
                .HasMaxLength(500);

            builder.Property(p => p.DateOfBirth)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.HasOne(p => p.ApplicationUser)
                .WithOne(u => u.ApplicationUserProfileDetails)
                .HasForeignKey<ApplicationUserProfileDetails>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
