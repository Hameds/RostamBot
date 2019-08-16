using Araye.Code.Cqrs.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RostamBot.Application.Features.Users.Models;
using RostamBot.Application.Interfaces;
using RostamBot.Application.Settings;
using RostamBot.Domain.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RostamBot.Application.Features.Users.Commands
{
    public class LoginUser : IRequest<LoginToken>
    {
        public string Username { get; set; }

        public string Password { get; set; }


        public class Handler : IRequestHandler<LoginUser, LoginToken>
        {
            private readonly IRostamBotDbContext _db;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly RostamBotSettings _rostamBotSettings;


            public Handler(IRostamBotDbContext db,
                           UserManager<ApplicationUser> userManager,
                           IOptions<RostamBotSettings> rostamBotSettings)
            {
                _db = db;
                _userManager = userManager;
                _rostamBotSettings = rostamBotSettings.Value;
            }

            public async Task<LoginToken> Handle(LoginUser request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(request.Username);

                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    throw new NotFoundException(nameof(ApplicationUser), request.Username);
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                var roles = String.Join(",", userRoles);

                var claims = new[] {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(ClaimTypes.Role,roles),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };


                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_rostamBotSettings.JwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                  issuer: _rostamBotSettings.JwtIssuer,
                  audience: _rostamBotSettings.JwtIssuer,
                  claims: claims,
                  expires: DateTime.Now.AddHours(1),
                  signingCredentials: creds);


                return new LoginToken() { Token = new JwtSecurityTokenHandler().WriteToken(token) };
            }
        }
    }
}
