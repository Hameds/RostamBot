using Microsoft.Extensions.Options;
using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Interfaces;
using RostamBot.Application.Settings;
using System;
using System.Collections.Generic;
using Tweetinvi;

namespace RostamBot.Infrastructure
{
    public class RostamBotService : IRostamBotService
    {
        private readonly RostamBotSettings _settings;


        public RostamBotService(IOptions<RostamBotSettings> settings)
        {
            _settings = settings.Value;

            if (!string.IsNullOrEmpty(_settings.TwitterProxy))
            {
                TweetinviConfig.ApplicationSettings.ProxyConfig = new ProxyConfig(_settings.TwitterProxy);
                TweetinviConfig.CurrentThreadSettings.ProxyConfig = new ProxyConfig(_settings.TwitterProxy);
            }

        }

        public Dictionary<SuspiciousAccountDto, bool> BlockUsers(string userAccessToken, string userAccessSecret, List<SuspiciousAccountDto> suspiciousAccountsToBlock, bool shouldBlock)
        {

            Auth.SetUserCredentials(
                                    _settings.TwitterAppConsumerKey,
                                    _settings.TwitterAppConsumerSecret,
                                    userAccessToken,
                                    userAccessSecret);

            var results = new Dictionary<SuspiciousAccountDto, bool>();

            foreach (var suspiciousAccount in suspiciousAccountsToBlock)
            {
                bool actionStatus = false;
                try
                {
                    //ToDo: checking block status on user profile before applying command is nice
                    if (shouldBlock)
                        actionStatus = User.BlockUser(suspiciousAccount.TwitterUserId);

                    //ToDo: for now we ignore unblockuser, it should be an option
                    //else
                    //    actionStatus = User.UnBlockUser(suspiciousAccount.TwitterUserId);
                }
                catch (Exception ex)
                {
                    //ToDo should handle RateLimit and log error
                }
                results.Add(suspiciousAccount, actionStatus);
            }

            return results;

        }
    }
}
