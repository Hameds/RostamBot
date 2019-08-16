using Hangfire;
using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Interfaces;
using RostamBot.Application.Notifications.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RostamBot.Infrastructure
{
    public class NotificationService : INotificationService
    {

        public Task SendRespondTweetAsync(RespondTweet tweet)
        {
            BackgroundJob.Enqueue<IRostamBotManagerService>(bot => bot.SendReply(tweet));

            return Task.CompletedTask;
        }

        public Task UpdateBlockList(SuspiciousAccountDto suspiciousAccount, bool shouldBlock)
        {
            BackgroundJob.Schedule<ISyncBlockListJob>(job => job.UpdateBlockList(suspiciousAccount, shouldBlock, new CancellationToken()), new DateTimeOffset(DateTime.Now.AddMinutes(15)));

            return Task.CompletedTask;
        }
    }
}
