using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurderGame.Entities.Domains
{
    namespace MurderGame.Entities.Domains
    {
        public class ApplicationUserMessage
        {
            public int MessageId { get; set; } // Primary Key
            public int SenderId { get; set; } // Foreign Key

            public string Content { get; set; }
            public DateTime SentAt { get; set; } = DateTime.UtcNow;
            public bool IsRead { get; set; } = false;

            // Navigation Property
            public ApplicationUser Sender { get; set; } // ✅ Kullanıcı ile 1-N ilişki
        }
    }

}
