using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RostamBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RostamBot.Persistence.Configurations
{
    public class SuspiciousAccountConfiguration : IEntityTypeConfiguration<SuspiciousAccount>
    {
        public void Configure(EntityTypeBuilder<SuspiciousAccount> builder)
        {

            builder.Property(e => e.TwitterScreenName)
                .IsRequired()
                .HasMaxLength(15);

        }
    }
}
