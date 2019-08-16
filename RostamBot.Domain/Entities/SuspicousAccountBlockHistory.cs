using System;

namespace RostamBot.Domain.Entities
{
    public class SuspicousAccountBlockHistory
    {
        public Guid Id { get; set; }

        public Guid SuspiciousAccountId { get; set; }

        public string UserId { get; set; }

        public bool IsBlocked { get; set; }

        public DateTime ActionDate { get; set; }


        public virtual SuspiciousAccount SuspiciousAccount { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}
