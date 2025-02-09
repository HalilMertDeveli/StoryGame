using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurderGame.Entities.Domains
{
    public class AdminActivityLog
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
        public string Action { get; set; }  // Örn: "User Ban"
        public string Description { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    }

}
