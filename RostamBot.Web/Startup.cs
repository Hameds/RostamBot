using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RostamBot.Web.Codes;
using System.Reflection;

namespace RostamBot.Web
{
    public class Startup
    {
        private readonly string _apiVersion = typeof(Startup).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRostamBotLibraries(Configuration, _apiVersion);

            services.AddRostamBotContext(Configuration);

            services.AddRostamBotIdentity(Configuration);

            services.AddRostamBotMvc(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRostamBotDefaultAppConfigurations(env);

            app.UseRostamBotLibraries(Configuration, _apiVersion);

            app.UseRostamBotMvc();
        }


    }
}
