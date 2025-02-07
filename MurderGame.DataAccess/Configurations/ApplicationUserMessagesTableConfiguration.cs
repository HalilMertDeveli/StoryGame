using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MurderGame.Entities.Domains;
using MurderGame.Entities.Domains.MurderGame.Entities.Domains;

namespace MurderGame.DataAccess.Configurations
{
    public class ApplicationUserMessagesTableConfiguration : IEntityTypeConfiguration<ApplicationUserMessage>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserMessage> builder)
        {
            builder.HasKey(m => m.MessageId);

            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(m => m.SentAt)
                .IsRequired();

            builder.Property(m => m.IsRead)
                .IsRequired();

            builder.HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
