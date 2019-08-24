using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Notifications.Models;
using System.Threading.Tasks;

namespace RostamBot.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendRespondTweetAsync(RespondTweet tweet);

        Task SendRespondDirectAsync(RespondDirect direct);

        Task UpdateBlockList(SuspiciousAccountDto suspiciousAccount, bool shouldBlock);
    }
}
