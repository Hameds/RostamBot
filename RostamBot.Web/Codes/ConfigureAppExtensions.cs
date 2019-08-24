using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using RostamBot.Application.Infrastructure.Hangfire;
using RostamBot.Application.Interfaces;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace RostamBot.Web.Codes
{
    public static class ConfigureAppExtensions
    {
        public static IApplicationBuilder UseRostamBotLibraries(this IApplicationBuilder app, IConfiguration configuration, string _apiVersion)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v{_apiVersion}/swagger.json", $"RostamBot API v{_apiVersion}");
                c.DocumentTitle = "RostamBot API";
                c.DocExpansion(DocExpansion.List);

            });


            var hangfireDashboardOptions = new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter(configuration) },
                IsReadOnlyFunc = (DashboardContext context) => true,
                DisplayStorageConnectionString = false,
                StatsPollingInterval = 30000
            };

            app.UseHangfireDashboard(configuration["RostamBotSettings:JobDashboardUrl"], hangfireDashboardOptions);

            //ToDo: remove magic numbers
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 2, DelaysInSeconds = new int[] { 1200, 1200 } });
            app.UseHangfireServer();

            RecurringJob.AddOrUpdate<ISyncReportsJob>(job => job.GetMentionsAsync(), Cron.MinuteInterval(20));
            RecurringJob.AddOrUpdate<ISyncReportsJob>(job => job.GetDirectsAsync(), Cron.MinuteInterval(20));


            return app;
        }

        public static IApplicationBuilder UseRostamBotDefaultAppConfigurations(this IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            return app;
        }

        public static IApplicationBuilder UseRostamBotMvc(this IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            return app;
        }
    }
}
