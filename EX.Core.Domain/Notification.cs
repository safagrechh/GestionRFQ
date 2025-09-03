using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EX.Core.Domain
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public string ActionUserName { get; set; } = string.Empty; // Name of user who performed the action

        // Link to RFQ
        public int RFQId { get; set; }
        public virtual RFQ RFQ { get; set; }   // <-- make it virtual

        // Link to User (engineer)
        public int UserId { get; set; }
        public virtual User User { get; set; } // <-- make it virtual
    }

}
