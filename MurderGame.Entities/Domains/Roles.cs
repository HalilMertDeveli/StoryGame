using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurderGame.Entities.Domains
{
    public class Role : IdentityRole<int> // int primary key kullanıyoruz
    {
        public string? Description { get; set; } // Rol Açıklaması
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Rol oluşturulma tarihi
    }
}
