using System;

namespace RostamBot.Domain.Entities
{
    public class Supervision
    {
        public Guid Id { get; set; }

        public Guid ModeratorId { get; set; }

        public Guid SuspiciousAccountId { get; set; }

        public bool? ShouldBlock { get; set; }


        public virtual SuspiciousAccount SuspiciousAccount { get; set; }

        public virtual Moderator Moderator { get; set; }


    }
}
