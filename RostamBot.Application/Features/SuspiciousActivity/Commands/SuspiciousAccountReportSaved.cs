using MediatR;
using RostamBot.Application.Interfaces;
using RostamBot.Application.Notifications.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RostamBot.Application.Features.SuspiciousActivity.Commands
{
    public class SuspiciousAccountReportSaved : INotification
    {
        public long ReporterTwitterUserId { get; set; }

        public long ReporterTweetId { get; set; }

        public string ReporterScreenName { get; set; }

        public string SuspiciousAccountScreenName { get; set; }

        public bool? IsSuspiciousAccountBlocked { get; set; }

        public bool ShouldRespondViaDirect { get; set; }


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

                if (notification.ShouldRespondViaDirect)
                {
                    var respondDirect = new RespondDirect()
                    {
                        ReceiverTwitterUserId = notification.ReporterTwitterUserId,
                        Text = respondText
                    };

                    await _notification.SendRespondDirectAsync(respondDirect);
                }

                else
                {
                    var respondTweet = new RespondTweet()
                    {
                        InReplyToTweetId = notification.ReporterTweetId,
                        InReplyToScreenName = notification.ReporterScreenName,
                        Text = respondText + " همچنین می‌توانید گزارشات خود را با ارسال توییت اکانت ناامن به صورت دایرکت به ما و بدون نیاز به هشتگ بفرستید."
                    };

                    await _notification.SendRespondTweetAsync(respondTweet);
                }

            }
        }
    }
}
