using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurderGame.Entities.Domains
{
    public class Admin
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // ApplicationUser FK
        public ApplicationUser User { get; set; }
        public string Level { get; set; }  // SuperAdmin, Moderator, vb.
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public ICollection<AdminActivityLog> AdminLogs { get; set; } = new List<AdminActivityLog>();
    }

}
