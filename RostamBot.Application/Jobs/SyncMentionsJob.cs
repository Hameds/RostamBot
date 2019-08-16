using MediatR;
using RostamBot.Application.Features.SuspiciousActivity.Commands;
using RostamBot.Application.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace RostamBot.Application.Jobs
{
    public class SyncMentionsJob : ISyncMentionsJob
    {
        private readonly IRostamBotDbContext _db;
        private readonly IMediator _mediator;
        private readonly IRostamBotManagerService _botManagerService;

        public SyncMentionsJob(IRostamBotDbContext db, IMediator mediator, IRostamBotManagerService botManagerService)
        {
            _db = db;
            _mediator = mediator;
            _botManagerService = botManagerService;
        }

        //ToDo: async this process
        public async Task GetMentionsAsync()
        {
            var lastProcessedMention = _db.SuspiciousAccountReports.DefaultIfEmpty().Max(x => x.TweetId);

            var latestMentions = _botManagerService.GetMentions(lastProcessedMention);


            foreach (var mention in latestMentions)
            {
                var newReport = new ReportSuspiciousActivity()
                {
                    ReporterTweetContent = mention.ReporterTweetContent,
                    ReporterTweetId = mention.ReporterTweetId,
                    ReporterTwitterScreenName = mention.ReporterTwitterScreenName,
                    ReporterTwitterUserId = mention.ReporterTwitterUserId,
                    SuspiciousAccountTwitterScreenName = mention.SuspiciousAccountTwitterScreenName,
                    SuspiciousAccountTwitterUserId = mention.SuspiciousAccountTwitterUserId,
                    SuspiciousAccountTwitterUserJoinDate = mention.SuspiciousAccountTwitterUserJoinDate,
                    SuspiciousTweetContent = mention.SuspiciousTweetContent,
                    SuspiciousTweetId = mention.SuspiciousTweetId
                };

                await _mediator.Send(newReport);
            }

        }
    }
}
