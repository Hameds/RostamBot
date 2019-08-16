using MediatR;
using RostamBot.Application.Interfaces;
using RostamBot.Application.Notifications.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RostamBot.Application.Features.SuspiciousActivity.Commands
{
    public class SuspiciousAccountReportSaved : INotification
    {
        public long ReporterTweetId { get; set; }

        public string ReporterScreenName { get; set; }

        public string SuspiciousAccountScreenName { get; set; }

        public bool? IsSuspiciousAccountBlocked { get; set; }


        public class SuspiciousAccountReportSavedHandler : INotificationHandler<SuspiciousAccountReportSaved>
        {
            private readonly INotificationService _notification;

            public SuspiciousAccountReportSavedHandler(INotificationService notification)
            {
                _notification = notification;
            }

            public async Task Handle(SuspiciousAccountReportSaved notification, CancellationToken cancellationToken)
            {
                string respondText =
                    notification.IsSuspiciousAccountBlocked.HasValue && notification.IsSuspiciousAccountBlocked.Value ?
                    $"ضمن تشکر، کاربر @{notification.SuspiciousAccountScreenName} قبلاً در لیست بلاک #رستم_بات قرار گرفته است." :
                    $"ضمن تشکر، گزارش شما برای کاربر @{notification.SuspiciousAccountScreenName} ثبت شد و به زودی در #رستم_بات مورد بررسی قرار خواهد گرفت.";

                var respondTweet = new RespondTweet()
                {
                    InReplyToTweetId = notification.ReporterTweetId,
                    InReplyToScreenName = notification.ReporterScreenName,
                    Text = respondText
                };

                await _notification.SendRespondTweetAsync(respondTweet);
            }
        }
    }
}
