using System;

namespace RostamBot.Application.Features.SuspiciousActivity.Models
{
    public class ReportDto
    {
        public long ReporterTwitterUserId { get; set; }

        public string ReporterTwitterScreenName { get; set; }

        public long ReporterTweetId { get; set; }

        public string ReporterTweetContent { get; set; }

        public long SuspiciousAccountTwitterUserId { get; set; }

        public string SuspiciousAccountTwitterScreenName { get; set; }

        public DateTime SuspiciousAccountTwitterUserJoinDate { get; set; }

        public long SuspiciousTweetId { get; set; }

        public string SuspiciousTweetContent { get; set; }

        public bool IsDirectMessage { get; set; }
    }
}
