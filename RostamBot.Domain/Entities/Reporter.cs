using System;
using System.Collections.Generic;

namespace RostamBot.Domain.Entities
{
    public class Reporter
    {
        public Reporter()
        {
            SuspiciousAccountReports = new HashSet<SuspiciousAccountReport>();
            SuspiciousTweets = new HashSet<SuspiciousTweet>();
        }


        public Guid Id { get; set; }

        public string TwitterScreenName { get; set; }

        public long TwitterUserId { get; set; }


        public virtual IEnumerable<SuspiciousAccountReport> SuspiciousAccountReports { get; private set; }
        public virtual IEnumerable<SuspiciousTweet> SuspiciousTweets { get; private set; }

    }
}
