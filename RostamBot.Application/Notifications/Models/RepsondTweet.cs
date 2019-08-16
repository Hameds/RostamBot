namespace RostamBot.Application.Notifications.Models
{
    public class RespondTweet
    {
        public long InReplyToTweetId { get; set; }

        public string InReplyToScreenName { get; set; }

        public string Text { get; set; }
    }
}
