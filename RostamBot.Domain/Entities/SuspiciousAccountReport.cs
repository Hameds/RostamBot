using System;

namespace RostamBot.Domain.Entities
{
    public class SuspiciousAccountReport
    {
        public Guid Id { get; set; }

        public Guid SuspiciousAccountId { get; set; }

        public Guid ReporterId { get; set; }

        public DateTime ReportDate { get; set; }

        public long TweetId { get; set; }

        public string TweetContent { get; set; }

        public bool IsViaDirect { get; set; }




        public virtual SuspiciousAccount SuspiciousAccount { get; set; }
        public virtual Reporter Reporter { get; set; }

    }
}
