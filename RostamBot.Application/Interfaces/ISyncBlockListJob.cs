using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace RostamBot.Application.Interfaces
{
    public interface ISyncBlockListJob
    {
        Task ApplyBlockListForFirstTime(ApplicationUser rostamBotUser, CancellationToken cancellationToken);

        Task UpdateBlockList(SuspiciousAccountDto suspiciousAccount, bool shouldBlock, CancellationToken cancellationToken);

    }
}
