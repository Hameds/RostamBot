using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RostamBot.Application.Interfaces;
using RostamBot.Application.Settings;
using RostamBot.Domain.Entities;
using RostamBot.Persistence;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RostamBot.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                try
                {

                    var context = scope.ServiceProvider.GetService<IRostamBotDbContext>();

                    var concreteContext = (RostamBotDbContext)context;

                    var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                    var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                    var settings = scope.ServiceProvider.GetService<IOptions<RostamBotSettings>>();

                    concreteContext.Database.Migrate();

                    var rostamBotDbInitializer = new RostamBotDbInitializer(userManager, roleManager, settings);

                    Task.Run(async () => { await rostamBotDbInitializer.InitializeAsync(); }).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or initializing the database.");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            new WebHostBuilder()
                .UseKestrel(options => options.AddServerHeader = false)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.Local.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                    config.AddUserSecrets<Startup>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseStartup<Startup>();
    }
}
