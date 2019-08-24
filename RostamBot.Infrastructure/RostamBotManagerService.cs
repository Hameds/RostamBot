using Hangfire;
using Microsoft.Extensions.Options;
using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Interfaces;
using RostamBot.Application.Notifications.Models;
using RostamBot.Application.Settings;
using System;
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

            if (!string.IsNullOrEmpty(_settings.TwitterProxy))
            {
                TweetinviConfig.ApplicationSettings.ProxyConfig = new ProxyConfig(_settings.TwitterProxy);
                TweetinviConfig.CurrentThreadSettings.ProxyConfig = new ProxyConfig(_settings.TwitterProxy);
            }


            Auth.SetUserCredentials(_settings.ManagerTwitterAppConsumerKey,
                                    _settings.ManagerTwitterAppConsumerSecret,
                                    _settings.ManagerTwitterAppUserAccessToken,
                                    _settings.ManagerTwitterAppUserAccessSecret);

        }

        public List<ReportDto> GetDirectMessages()
        {

            //ToDo: remove magic numbers also this code means that we always get latest 300 directs!
            var messages = Message.GetLatestMessages(new GetMessagesParameters()
            {
                Count = 150
            }, out var nextCursor).ToList();

            if (nextCursor != null)
            {
                var olderMessages = Message.GetLatestMessages(new GetMessagesParameters()
                {
                    Count = 150,
                    Cursor = nextCursor

                });

                messages.AddRange(olderMessages);
            }


            var model = new List<ReportDto>();

            //ToDo: I cry everytime that I see this logic, refactor it as soon as possible :(
            if (messages != null)
            {
                var rostamBotUser = User.GetUserFromScreenName("RostamBot");

                var messagesWithUrl = messages.Where(x => x.SenderId != rostamBotUser.Id && x.Entities.Urls.Count == 1);

                foreach (var directMessage in messagesWithUrl)
                {
                    try
                    {
                        var reporterUser = User.GetUserFromId(directMessage.SenderId);

                        var directMessageUrl = directMessage.Entities.Urls.FirstOrDefault().ExpandedURL.ToLower();


                        if (directMessageUrl.Contains("twitter.com") && directMessageUrl.Contains("status"))
                        {
                            var reportedTweetId = long.Parse(directMessageUrl.Split('/').Last());
                            var reportedTweet = Tweet.GetTweet(reportedTweetId);

                            if (reportedTweet != null)
                            {
                                var modelItem = new ReportDto()
                                {
                                    ReporterTweetId = directMessage.Id,
                                    ReporterTweetContent = directMessage.Text,
                                    ReporterTwitterScreenName = reporterUser.ScreenName,
                                    ReporterTwitterUserId = directMessage.SenderId,
                                    SuspiciousAccountTwitterScreenName = reportedTweet.CreatedBy.ScreenName,
                                    SuspiciousAccountTwitterUserId = reportedTweet.CreatedBy.Id,
                                    SuspiciousTweetContent = reportedTweet.Text,
                                    SuspiciousTweetId = reportedTweet.Id,
                                    SuspiciousAccountTwitterUserJoinDate = reportedTweet.CreatedBy.CreatedAt,
                                    IsDirectMessage = true
                                };
                                model.Add(modelItem);
                            }

                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                        //ToDo: but we need to log it
                    }

                }
            }

            return model;


        }

        //ToDo: make this async
        public List<ReportDto> GetMentions(long lastProcessedTweetId)
        {

            var mentionsTimelineParameters = new MentionsTimelineParameters
            {
                MaximumNumberOfTweetsToRetrieve = 100,

            };



            //ToDo: remove magic number (it's default value of long for the first run)
            if (lastProcessedTweetId != 0)
                mentionsTimelineParameters.SinceId = lastProcessedTweetId;



            var mentions = Timeline.GetMentionsTimeline(mentionsTimelineParameters);

            var model = new List<ReportDto>();
            if (mentions != null)
            {
                //ToDo: should remove magic string and read it from settings (#naamn)
                foreach (var mention in mentions.Where(x => x.Hashtags.Any(h => h.Text == "naamn")))
                {
                    var modelItem = new ReportDto()
                    {
                        ReporterTweetId = mention.Id,
                        ReporterTweetContent = mention.Text,
                        ReporterTwitterScreenName = mention.CreatedBy.ScreenName,
                        ReporterTwitterUserId = mention.CreatedBy.Id,
                        SuspiciousAccountTwitterScreenName = mention.QuotedTweet != null ? mention.QuotedTweet.CreatedBy.ScreenName : mention.InReplyToScreenName,
                        SuspiciousAccountTwitterUserId = mention.QuotedTweet != null ? mention.QuotedTweet.CreatedBy.Id : mention.InReplyToUserId.Value,
                        SuspiciousTweetContent = mention.QuotedTweet != null ? mention.QuotedTweet.Text : Tweet.GetTweet(mention.InReplyToStatusId.Value).Text,
                        SuspiciousTweetId = mention.QuotedTweet != null ? mention.QuotedTweet.Id : mention.InReplyToStatusId.Value,
                        SuspiciousAccountTwitterUserJoinDate = mention.QuotedTweet != null ? mention.QuotedTweet.CreatedBy.CreatedAt : User.GetUserFromId(mention.InReplyToUserId.Value).CreatedAt,
                        IsDirectMessage = false
                    };
                    model.Add(modelItem);
                }
            }

            return model;


        }

        public void SendReplyDirect(RespondDirect direct)
        {
            BackgroundJob.Enqueue(() =>
                Message.PublishMessage(direct.Text, direct.ReceiverTwitterUserId)
            );
        }

        public void SendReplyTweet(RespondTweet tweet)
        {
            var textToPublish = string.Format($"@{tweet.InReplyToScreenName} {tweet.Text}");

            BackgroundJob.Enqueue(() =>
                Tweet.PublishTweetInReplyTo(textToPublish, tweet.InReplyToTweetId)
            );

        }
    }
}
