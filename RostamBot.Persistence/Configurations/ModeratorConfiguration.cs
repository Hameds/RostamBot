using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RostamBot.Domain.Entities;

namespace RostamBot.Persistence.Configurations
{
    public class ModeratorConfiguration : IEntityTypeConfiguration<Moderator>
    {
        public void Configure(EntityTypeBuilder<Moderator> builder)
        {

            builder.Property(e => e.TwitterScreenName)
                .IsRequired()
                .HasMaxLength(15);

        }
    }
}
