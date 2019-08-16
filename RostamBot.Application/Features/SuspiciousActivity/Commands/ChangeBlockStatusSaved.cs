using MediatR;
using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace RostamBot.Application.Features.SuspiciousActivity.Commands
{
    public class ChangeBlockStatusSaved : INotification
    {
        public SuspiciousAccountDto SuspiciousAccountDto { get; set; }

        public bool BlockStatus { get; set; }


        public class SuspiciousAccountReportSavedHandler : INotificationHandler<ChangeBlockStatusSaved>
        {
            private readonly INotificationService _notification;

            public SuspiciousAccountReportSavedHandler(INotificationService notification)
            {
                _notification = notification;
            }

            public async Task Handle(ChangeBlockStatusSaved notification, CancellationToken cancellationToken)
            {
                await _notification.UpdateBlockList(notification.SuspiciousAccountDto, notification.BlockStatus);
            }
        }
    }
}
