using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RostamBot.Domain.Entities;

namespace RostamBot.Persistence.Configurations
{
    public class ReporterConfiguration : IEntityTypeConfiguration<Reporter>
    {
        public void Configure(EntityTypeBuilder<Reporter> builder)
        {

            builder.Property(e => e.TwitterScreenName)
                .IsRequired()
                .HasMaxLength(15);

        }
    }
}
