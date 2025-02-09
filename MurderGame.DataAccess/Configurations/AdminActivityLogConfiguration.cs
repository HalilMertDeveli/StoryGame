using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MurderGame.Entities.Domains;

namespace MurderGame.DataAccess.Configurations
{
    public class AdminActivityLogConfiguration : IEntityTypeConfiguration<AdminActivityLog>
    {
        public void Configure(EntityTypeBuilder<AdminActivityLog> builder)
        {
            builder.ToTable("AdminActivityLogs");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(x => x.Admin)
                .WithMany(x => x.AdminLogs)
                .HasForeignKey(x => x.AdminId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
