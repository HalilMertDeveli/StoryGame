using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurderGame.Entities.Domains
{
    namespace MurderGame.Entities.Domains
    {
        public class ApplicationUserProfileDetails
        {
            public int ProfileDetailsId { get; set; } // Primary Key
            public int UserId { get; set; } // Foreign Key

            public string? DisplayName { get; set; }
            public string? ProfilePicture { get; set; }
            public string? Bio { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string? Location { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }

            // Navigation Property
            public ApplicationUser ApplicationUser { get; set; } 
        }
    }

}
