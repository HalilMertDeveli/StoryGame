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

        public DbSet<Admin> Admins { get; set; }
        public DbSet<AdminActivityLog> AdminActivityLogs { get; set; }
        public DbSet<ApplicationUserMessage> UserMessagesTable { get; set; }
        public DbSet<ApplicationUserProfileDetails> UserProfileDetailsTable { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ApplicationUserTableConfiguration());
            builder.ApplyConfiguration(new ApplicationUserProfileTableConfiguration());
            builder.ApplyConfiguration(new ApplicationUserMessagesTableConfiguration());
            builder.ApplyConfiguration(new AdminConfiguration());  // Admin Configuration
            builder.ApplyConfiguration(new AdminActivityLogConfiguration());  // Admin Activity Configuration
            builder.ApplyConfiguration(new RolesTableConfiguration());

            // ApplicationUser ve ApplicationUserProfileDetails arasında 1-1 ilişki
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.ApplicationUserProfileDetails)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<ApplicationUserProfileDetails>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser ve ApplicationUserMessage arasında 1-N ilişki
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Messages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }


}



