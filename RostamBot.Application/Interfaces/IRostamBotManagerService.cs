using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Notifications.Models;
using System.Collections.Generic;

namespace RostamBot.Application.Interfaces
{
    public interface IRostamBotManagerService
    {
        List<MentionDto> GetMentions(long lastProcessedTweetId);

        void SendReply(RespondTweet tweet);
    }
}
