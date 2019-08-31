using System;

namespace RostamBot.Application.Features.SuspiciousActivity.Models
{
    public class SuspiciousAccountDto
    {
        public Guid Id { get; set; }

        public long TwitterUserId { get; set; }

        public string TwitterScreenName { get; set; }

        public DateTime TwitterJoinDate { get; set; }
    }
}
