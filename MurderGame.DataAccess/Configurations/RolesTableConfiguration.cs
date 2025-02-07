using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MurderGame.Entities.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurderGame.DataAccess.Configurations
{
    public class RolesTableConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id); // IdentityRole<int> zaten Id içeriyor

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.Name).IsUnique(); // RoleName benzersiz olmalı

            builder.Property(r => r.Description)
                .HasMaxLength(255);

            builder.Property(r => r.CreatedAt)
                .IsRequired();
        }
    }

}
