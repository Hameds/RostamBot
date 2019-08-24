using MediatR;
using RostamBot.Application.Features.SuspiciousActivity.Commands;
using RostamBot.Application.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace RostamBot.Application.Jobs
{
    public class SyncReportsJob : ISyncReportsJob
    {
        private readonly IRostamBotDbContext _db;
        private readonly IMediator _mediator;
        private readonly IRostamBotManagerService _botManagerService;

        public SyncReportsJob(IRostamBotDbContext db, IMediator mediator, IRostamBotManagerService botManagerService)
        {
            _db = db;
            _mediator = mediator;
            _botManagerService = botManagerService;
        }

        public async Task GetDirectsAsync()
        {
            var latestDirects = _botManagerService.GetDirectMessages();

            foreach (var direct in latestDirects)
            {
                var newReport = new ReportSuspiciousActivity()
                {
                    ReporterTweetContent = direct.ReporterTweetContent,
                    ReporterTweetId = direct.ReporterTweetId,
                    ReporterTwitterScreenName = direct.ReporterTwitterScreenName,
                    ReporterTwitterUserId = direct.ReporterTwitterUserId,
                    SuspiciousAccountTwitterScreenName = direct.SuspiciousAccountTwitterScreenName,
                    SuspiciousAccountTwitterUserId = direct.SuspiciousAccountTwitterUserId,
                    SuspiciousAccountTwitterUserJoinDate = direct.SuspiciousAccountTwitterUserJoinDate,
                    SuspiciousTweetContent = direct.SuspiciousTweetContent,
                    SuspiciousTweetId = direct.SuspiciousTweetId,
                    IsViaDirect = true
                };

                await _mediator.Send(newReport);
            }

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
                    SuspiciousTweetId = mention.SuspiciousTweetId,
                    IsViaDirect = false
                };

                await _mediator.Send(newReport);
            }

        }
    }
}
