using Microsoft.EntityFrameworkCore;
using RostamBot.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RostamBot.Application.Interfaces
{
    public interface IRostamBotDbContext : IDisposable
    {
        DbSet<ApplicationUser> ApplicationUsers { get; set; }

        DbSet<Moderator> Moderators { get; set; }

        DbSet<Reporter> Reporters { get; set; }

        DbSet<Supervision> Supervisions { get; set; }

        DbSet<SuspiciousAccount> SuspiciousAccounts { get; set; }

        DbSet<SuspiciousAccountReport> SuspiciousAccountReports { get; set; }

        DbSet<SuspiciousTweet> SuspiciousTweets { get; set; }

        DbSet<SuspicousAccountBlockHistory> SuspicousAccountBlockHistories { get; set; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
