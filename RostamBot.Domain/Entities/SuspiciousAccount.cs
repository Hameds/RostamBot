using System;
using System.Collections.Generic;

namespace RostamBot.Domain.Entities
{
    public class SuspiciousAccount
    {
        public SuspiciousAccount()
        {
            SuspiciousAccountReports = new HashSet<SuspiciousAccountReport>();
            Supervisions = new HashSet<Supervision>();
            SuspiciousTweets = new HashSet<SuspiciousTweet>();
            SuspicousAccountBlockHistories = new HashSet<SuspicousAccountBlockHistory>();
        }


        public Guid Id { get; set; }

        public string TwitterScreenName { get; set; }

        public long TwitterUserId { get; set; }

        public DateTime TwitterJoinDate { get; set; }

        public bool? ShouldBlock { get; set; }


        public virtual IEnumerable<SuspiciousAccountReport> SuspiciousAccountReports { get; private set; }
        public virtual IEnumerable<Supervision> Supervisions { get; private set; }
        public virtual IEnumerable<SuspiciousTweet> SuspiciousTweets { get; private set; }
        public virtual IEnumerable<SuspicousAccountBlockHistory> SuspicousAccountBlockHistories { get; private set; }



    }
}
