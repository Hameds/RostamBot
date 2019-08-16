using Araye.Code.Cqrs.Application.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RostamBot.Application.Settings;
using RostamBot.Domain.Entities;
using System.Threading.Tasks;

namespace RostamBot.Persistence
{
    public class RostamBotDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RostamBotSettings _settings;



        public RostamBotDbInitializer(
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                IOptions<RostamBotSettings> rostamBotSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _settings = rostamBotSettings.Value;
        }

        public async Task InitializeAsync()
        {
            await CreateDefaultAdminRole();

            await CreateDefaultAdminUser();

        }

        private async Task CreateDefaultAdminRole()
        {
            if (await _roleManager.FindByNameAsync(_settings.DefaultAdminRole) == null)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole()
                {
                    Name = _settings.DefaultAdminRole,
                    NormalizedName = _settings.DefaultAdminRole.ToUpper(),
                });

                if (!result.Succeeded)
                {
                    throw new AppException("Failed to seed admin default role");
                }
            }
        }

        private async Task CreateDefaultAdminUser()
        {
            if (await _userManager.FindByEmailAsync(_settings.DefaultAdminEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = _settings.DefaultAdminEmail,
                    Email = _settings.DefaultAdminEmail,
                };

                var result = await _userManager.CreateAsync(user, password: _settings.DefaultAdminPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role: _settings.DefaultAdminRole);
                }
                else
                {
                    throw new AppException("Failed to seed admin default user");
                }
            }
        }


    }
}
