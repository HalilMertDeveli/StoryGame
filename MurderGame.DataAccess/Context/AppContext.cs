using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MurderGame.DataAccess.Configurations;
using MurderGame.Entities.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MurderGame.Entities.Domains.MurderGame.Entities.Domains;

namespace MurderGame.DataAccess.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, Role, int>
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

      

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ApplicationUserTableConfiguration());
            builder.ApplyConfiguration(new ApplicationUserProfileTableConfiguration());
            builder.ApplyConfiguration(new ApplicationUserMessagesTableConfiguration());
            builder.ApplyConfiguration(new RolesTableConfiguration());

            // ApplicationUser ve ApplicationUserProfileDetails arasında 1-1 ilişki
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.ApplicationUserProfileDetails)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<ApplicationUserProfileDetails>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silinirse profili de silinsin

            // ApplicationUser ve ApplicationUserMessage arasında 1-N ilişki
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Messages)
                .WithOne(m => m.Sender) // ❌ Hatalı: .WithOne(m => m.SenderId) yerine .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade); // Kullanıcı silinirse mesajları da silinsin
        }
        public DbSet<ApplicationUserMessage> UserMessagesTable { get; set; }
        public DbSet<ApplicationUserProfileDetails> UserProfileDetailsTable { get; set; }
    }

}



