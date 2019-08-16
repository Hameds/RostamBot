using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RostamBot.Application.Settings;
using RostamBot.Persistence.Infrastructure;

namespace RostamBot.Persistence
{
    public class RostamBotDbContextFactory : DesignTimeDbContextFactoryBase<RostamBotDbContext>
    {

        public RostamBotDbContextFactory()
        {

        }

       
        protected override RostamBotDbContext CreateNewInstance(DbContextOptions<RostamBotDbContext> options)
        {
            return new RostamBotDbContext(options);
        }
    }
}
