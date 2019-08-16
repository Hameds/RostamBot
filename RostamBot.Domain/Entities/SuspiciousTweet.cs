using System;

namespace RostamBot.Domain.Entities
{
    public class SuspiciousTweet
    {
        public Guid Id { get; set; }

        public Guid SuspiciousAccountId { get; set; }

        public Guid ReporterId { get; set; }

        public long TweetId { get; set; }

        public string TweetContent { get; set; }



        public virtual SuspiciousAccount SuspiciousAccount { get; set; }
        public virtual Reporter Reporter { get; set; }
    }
}
