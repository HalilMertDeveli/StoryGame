using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MurderGame.Entities.Domains.MurderGame.Entities.Domains;

namespace MurderGame.Entities.Domains
{
    public class ApplicationUser : IdentityUser<int> // int primary key kullanıyoruz
    {
        public string? UserNickName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public bool IsVerified { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }

        // Navigation Properties
        public ICollection<ApplicationUserMessage> Messages { get; set; } = new List<ApplicationUserMessage>(); // ✅ HATA DÜZELTİLDİ
        public ApplicationUserProfileDetails ApplicationUserProfileDetails { get; set; }
    }
}
