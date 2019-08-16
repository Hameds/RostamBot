using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RostamBot.Domain.Entities;

namespace RostamBot.Persistence.Configurations
{
    public class SuspiciousTweetConfiguration : IEntityTypeConfiguration<SuspiciousTweet>
    {
        public void Configure(EntityTypeBuilder<SuspiciousTweet> builder)
        {


            builder.Property(e => e.TweetContent)
               .IsRequired();

        }
    }
}
