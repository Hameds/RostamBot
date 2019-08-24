using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Notifications.Models;
using System.Collections.Generic;

namespace RostamBot.Application.Interfaces
{
    public interface IRostamBotManagerService
    {
        List<ReportDto> GetMentions(long lastProcessedTweetId);

        List<ReportDto> GetDirectMessages();

        void SendReplyTweet(RespondTweet tweet);

        void SendReplyDirect(RespondDirect tweet);

    }
}
