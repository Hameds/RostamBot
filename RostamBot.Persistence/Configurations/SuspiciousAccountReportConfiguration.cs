using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RostamBot.Domain.Entities;

namespace RostamBot.Persistence.Configurations
{
    public class SuspiciousAccountReportConfiguration : IEntityTypeConfiguration<SuspiciousAccountReport>
    {
        public void Configure(EntityTypeBuilder<SuspiciousAccountReport> builder)
        {

            builder.Property(e => e.TweetContent)
               .IsRequired();

        }
    }
}
