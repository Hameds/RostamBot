using Araye.Code.Cqrs.WebApi;
using AutoMapper;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using RostamBot.Application.Features.Users.Commands;
using RostamBot.Application.Infrastructure.AutoMapper;
using RostamBot.Application.Infrastructure.Hangfire;
using RostamBot.Application.Interfaces;
using RostamBot.Application.Jobs;
using RostamBot.Application.Settings;
using RostamBot.Domain.Entities;
using RostamBot.Infrastructure;
using RostamBot.Persistence;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace RostamBot.Web.Codes
{
    public static class ConfigureServiceExtensions
    {
        public static IServiceCollection AddRostamBotLibraries(this IServiceCollection services, IConfiguration configuration, string apiVersion)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile).GetTypeInfo().Assembly);

            services.AddMediatR(typeof(LoginUser).GetTypeInfo().Assembly);


            services.AddHangfire(x => x.UseSqlServerStorage(configuration["RostamBotSettings:DatabaseConnectionString"],
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.FromMinutes(30),
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                }));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{apiVersion}", new Info
                {
                    Title = "RostamBot APIs",
                    Version = $"v{apiVersion}",
                    Description = "RostamBot Backend APIs",
                    Contact = new Contact
                    {
                        Name = "RostamBot",
                        Email = "hi@rostambot.com",
                        Url = "https://rostambot.com"
                    }
                });


                c.DescribeAllEnumsAsStrings();

                // https://github.com/domaindrivendev/Swashbuckle/issues/142
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);

            });

            return services;
        }

        public static IServiceCollection AddRostamBotContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RostamBotDbContext>(options =>
                 options.UseSqlServer(configuration["RostamBotSettings:DatabaseConnectionString"]));


            services.AddScoped<IRostamBotDbContext, RostamBotDbContext>();

            return services;
        }

        public static IServiceCollection AddRostamBotIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<RoleManager<IdentityRole>>();


            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<RostamBotDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                    .AddCookie(cookieOptions =>
                    {
                        cookieOptions.Cookie.Name = "RostamBotCookie";
                    })
                    .AddTwitter(twitterOptions =>
                    {
                        twitterOptions.SaveTokens = true;
                        twitterOptions.ConsumerKey = configuration["RostamBotSettings:TwitterAppConsumerKey"];
                        twitterOptions.ConsumerSecret = configuration["RostamBotSettings:TwitterAppConsumerSecret"];
                        twitterOptions.BackchannelHttpHandler = new HttpClientHandler()
                        {
                            Proxy = new WebProxy(configuration["RostamBotSettings:TwitterProxy"])
                        };
                    })
                    .AddJwtBearer(config =>
                    {
                        config.RequireHttpsMetadata = false;
                        config.SaveToken = true;

                        config.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidIssuer = configuration["RostamBotSettings:JwtIssuer"],
                            ValidAudience = configuration["RostamBotSettings:JwtIssuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["RostamBotSettings:JwtKey"]))
                        };
                    });


            services.AddCors(options =>
            {


                options.AddPolicy("AccessPolicy",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });

            });


            return services;
        }

        public static IServiceCollection AddRostamBotMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            services.Configure<RostamBotSettings>(configuration.GetSection("RostamBotSettings"));
            //ToDo: add proper cycle
            services.AddTransient<INotificationService, NotificationService>();
            services.AddScoped<ICommandsExecutor, CommandsExecutor>();
            services.AddScoped<ICommandsScheduler, CommandsScheduler>();
            services.AddTransient<IRostamBotManagerService, RostamBotManagerService>();
            services.AddTransient<IRostamBotService, RostamBotService>();

            services.AddScoped<ISyncReportsJob, SyncReportsJob>();
            services.AddScoped<ISyncBlockListJob, SyncBlockListJob>();


            services
                .AddMvc(options => options.Filters.Add(typeof(CustomExceptionFilterAttribute)))
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginUserCommandValidator>());



            return services;
        }

    }
}
