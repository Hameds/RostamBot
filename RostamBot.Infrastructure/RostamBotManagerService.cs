using Hangfire;
using Microsoft.Extensions.Options;
using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Interfaces;
using RostamBot.Application.Notifications.Models;
using RostamBot.Application.Settings;
using System.Collections.Generic;
using System.Linq;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace RostamBot.Infrastructure
{
    public class RostamBotManagerService : IRostamBotManagerService
    {
        private readonly RostamBotSettings _settings;

        public RostamBotManagerService(IOptions<RostamBotSettings> settings)
        {
            _settings = settings.Value;
        }

        //ToDo: make this async
        public List<MentionDto> GetMentions(long lastProcessedTweetId)
        {
            //ToDo: should refator this to Utility Method
            if (!string.IsNullOrEmpty(_settings.TwitterProxy))
            {
                TweetinviConfig.ApplicationSettings.ProxyConfig = new ProxyConfig(_settings.TwitterProxy);
                TweetinviConfig.CurrentThreadSettings.ProxyConfig = new ProxyConfig(_settings.TwitterProxy);
            }


            Auth.SetUserCredentials(
                                    _settings.ManagerTwitterAppConsumerKey,
                                    _settings.ManagerTwitterAppConsumerSecret,
                                    _settings.ManagerTwitterAppUserAccessToken,
                                    _settings.ManagerTwitterAppUserAccessSecret);



            var mentionsTimelineParameters = new MentionsTimelineParameters
            {
                MaximumNumberOfTweetsToRetrieve = 100,

            };



            //ToDo: remove magic number (it's default value of long for the first run)
            if (lastProcessedTweetId != 0)
                mentionsTimelineParameters.SinceId = lastProcessedTweetId;



            var mentions = Timeline.GetMentionsTimeline(mentionsTimelineParameters);

            var model = new List<MentionDto>();
            if (mentions != null)
            {
                //ToDo: should remove magic string and read it from settings (#naamn)
                foreach (var mention in mentions.Where(x => x.Hashtags.Any(h => h.Text == "naamn")))
                {
                    var modelItem = new MentionDto()
                    {
                        ReporterTweetId = mention.Id,
                        ReporterTweetContent = mention.Text,
                        ReporterTwitterScreenName = mention.CreatedBy.ScreenName,
                        ReporterTwitterUserId = mention.CreatedBy.Id,
                        SuspiciousAccountTwitterScreenName = mention.QuotedTweet != null ? mention.QuotedTweet.CreatedBy.ScreenName : mention.InReplyToScreenName,
                        SuspiciousAccountTwitterUserId = mention.QuotedTweet != null ? mention.QuotedTweet.CreatedBy.Id : mention.InReplyToUserId.Value,
                        SuspiciousTweetContent = mention.QuotedTweet != null ? mention.QuotedTweet.Text : Tweet.GetTweet(mention.InReplyToStatusId.Value).Text,
                        SuspiciousTweetId = mention.QuotedTweet != null ? mention.QuotedTweet.Id : mention.InReplyToStatusId.Value,
                        SuspiciousAccountTwitterUserJoinDate = mention.QuotedTweet != null ? mention.QuotedTweet.CreatedBy.CreatedAt : User.GetUserFromId(mention.InReplyToUserId.Value).CreatedAt
                    };
                    model.Add(modelItem);
                }
            }

            return model;


        }

        public void SendReply(RespondTweet tweet)
        {
            if (!string.IsNullOrEmpty(_settings.TwitterProxy))
            {
                TweetinviConfig.ApplicationSettings.ProxyConfig = new ProxyConfig(_settings.TwitterProxy);
                TweetinviConfig.CurrentThreadSettings.ProxyConfig = new ProxyConfig(_settings.TwitterProxy);
            }


            Auth.SetUserCredentials(_settings.ManagerTwitterAppConsumerKey,
                                                                       _settings.ManagerTwitterAppConsumerSecret,
                                                                       _settings.ManagerTwitterAppUserAccessToken,
                                                                       _settings.ManagerTwitterAppUserAccessSecret);



            var textToPublish = string.Format($"@{tweet.InReplyToScreenName} {tweet.Text}");

            BackgroundJob.Enqueue(() =>
                Tweet.PublishTweetInReplyTo(textToPublish, tweet.InReplyToTweetId)
            );

        }
    }
}
