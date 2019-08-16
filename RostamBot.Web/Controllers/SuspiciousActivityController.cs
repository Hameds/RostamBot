﻿using Araye.Code.Cqrs.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RostamBot.Application.Features.SuspiciousActivity.Commands;
using RostamBot.Application.Settings;
using System.Threading.Tasks;
using Tweetinvi;

namespace RostamBot.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SuspiciousActivityController : BaseApiController
    {
        private readonly RostamBotSettings _settings;


        public SuspiciousActivityController(IOptions<RostamBotSettings> settings)
        {
            _settings = settings.Value;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddSuspiciousTwitterAccount([FromBody]string twitterScreenName)
        {
            TweetinviConfig.ApplicationSettings.ProxyConfig = new ProxyConfig(_settings.TwitterProxy);
            TweetinviConfig.CurrentThreadSettings.ProxyConfig = new ProxyConfig(_settings.TwitterProxy);


            var userCredentials = Auth.SetUserCredentials(
              _settings.ManagerTwitterAppConsumerKey,
              _settings.ManagerTwitterAppConsumerSecret,
              _settings.ManagerTwitterAppUserAccessToken,
              _settings.ManagerTwitterAppUserAccessSecret);

            var suspiciousTwitterAccount = Tweetinvi.User.GetUserFromScreenName(twitterScreenName);

            if (suspiciousTwitterAccount == null)
            {
                return BadRequest();
            }

            var addSuspiciousAccount = new AddSuspiciousAccount()
            {
                TwitterJoinDate = suspiciousTwitterAccount.CreatedAt,
                TwitterScreenName = suspiciousTwitterAccount.ScreenName,
                TwitterUserId = suspiciousTwitterAccount.Id
            };
            return Ok(await Mediator.Send(addSuspiciousAccount));
        }

    }
}
