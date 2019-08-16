using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RostamBot.Application.Interfaces;
using RostamBot.Domain.Entities;

namespace RostamBot.Persistence
{
    public class RostamBotDbContext : IdentityDbContext<ApplicationUser>, IRostamBotDbContext
    {

        public RostamBotDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Moderator> Moderators { get; set; }

        public DbSet<Reporter> Reporters { get; set; }

        public DbSet<Supervision> Supervisions { get; set; }

        public DbSet<SuspiciousAccount> SuspiciousAccounts { get; set; }

        public DbSet<SuspiciousAccountReport> SuspiciousAccountReports { get; set; }

        public DbSet<SuspiciousTweet> SuspiciousTweets { get; set; }

        public DbSet<SuspicousAccountBlockHistory> SuspicousAccountBlockHistories { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(RostamBotDbContext).Assembly);

            foreach (var entity in builder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.Relational().TableName.Replace("AspNet", "");
            }
        }


    }
}
