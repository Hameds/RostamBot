using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using RostamBot.Application.Interfaces;
using RostamBot.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RostamBot.Application.Features.SuspiciousActivity.Commands
{
    public class ReportSuspiciousActivity : IRequest
    {
        public long ReporterTwitterUserId { get; set; }

        public string ReporterTwitterScreenName { get; set; }

        public long ReporterTweetId { get; set; }

        public string ReporterTweetContent { get; set; }

        public long SuspiciousAccountTwitterUserId { get; set; }

        public string SuspiciousAccountTwitterScreenName { get; set; }

        public DateTime SuspiciousAccountTwitterUserJoinDate { get; set; }

        public long SuspiciousTweetId { get; set; }

        public string SuspiciousTweetContent { get; set; }

        public bool IsViaDirect { get; set; }


        public class Handler : IRequestHandler<ReportSuspiciousActivity, Unit>
        {
            private readonly IRostamBotDbContext _db;
            private readonly IMediator _mediator;
            private readonly IHostingEnvironment _env;


            public Handler(IRostamBotDbContext db, IMediator mediator, IHostingEnvironment env)
            {
                _db = db;
                _mediator = mediator;
                _env = env;
            }

            public async Task<Unit> Handle(ReportSuspiciousActivity request, CancellationToken cancellationToken)
            {
                var reporter = await GetOrAddReporter(request, cancellationToken);

                var suspiciousAccount = await GetOrAddSuspiciousAccount(request, cancellationToken);

                await GetOrAddSuspiciousTweet(request, reporter, suspiciousAccount, cancellationToken);

                var suspiciousAccountReportExists = await _db.SuspiciousAccountReports.AnyAsync(x => x.TweetId == request.ReporterTweetId && x.IsViaDirect == request.IsViaDirect && x.ReporterId == reporter.Id);

                if (!suspiciousAccountReportExists)
                {
                    await CreateNewSuspiciousAccountReport(request, reporter, suspiciousAccount, cancellationToken);

                    // We reply to users only in Production Environment
                    if (_env.IsProduction())
                    {
                        await _mediator.Publish(
                        new SuspiciousAccountReportSaved
                        {
                            ReporterTweetId = request.ReporterTweetId,
                            IsSuspiciousAccountBlocked = suspiciousAccount.ShouldBlock,
                            SuspiciousAccountScreenName = request.SuspiciousAccountTwitterScreenName,
                            ReporterScreenName = request.ReporterTwitterScreenName,
                            ShouldRespondViaDirect = request.IsViaDirect,
                            ReporterTwitterUserId = request.ReporterTwitterUserId
                        },
                        cancellationToken);
                    }

                }

                return Unit.Value;
            }

            private async Task GetOrAddSuspiciousTweet(ReportSuspiciousActivity request, Reporter reporter, SuspiciousAccount suspiciousAccount, CancellationToken cancellationToken)
            {
                var suspiciousTweetExists = await _db.SuspiciousTweets.AnyAsync(x => x.TweetId == request.SuspiciousTweetId && x.ReporterId == reporter.Id);

                if (!suspiciousTweetExists)
                {
                    await CreateNewSuspiciousTweet(request, reporter, suspiciousAccount, cancellationToken);
                }
            }

            private async Task<SuspiciousAccount> GetOrAddSuspiciousAccount(ReportSuspiciousActivity request, CancellationToken cancellationToken)
            {
                var suspiciousAccount = await _db.SuspiciousAccounts.SingleOrDefaultAsync(x => x.TwitterUserId == request.SuspiciousAccountTwitterUserId);
                if (suspiciousAccount == null)
                {
                    suspiciousAccount = await CreateNewSuspiciousAccount(request, cancellationToken);
                }

                return suspiciousAccount;
            }

            private async Task<Reporter> GetOrAddReporter(ReportSuspiciousActivity request, CancellationToken cancellationToken)
            {
                var reporter = await _db.Reporters.SingleOrDefaultAsync(x => x.TwitterUserId == request.ReporterTwitterUserId);
                if (reporter == null)
                {
                    reporter = await CreateNewReporter(request, cancellationToken);
                }

                return reporter;
            }

            private async Task CreateNewSuspiciousAccountReport(ReportSuspiciousActivity request, Reporter reporter, SuspiciousAccount suspiciousAccount, CancellationToken cancellationToken)
            {
                var suspiciousAccountReport = new SuspiciousAccountReport()
                {
                    ReportDate = DateTime.Now,
                    ReporterId = reporter.Id,
                    SuspiciousAccountId = suspiciousAccount.Id,
                    TweetId = request.ReporterTweetId,
                    TweetContent = request.ReporterTweetContent,
                    IsViaDirect = request.IsViaDirect
                };

                await _db.SuspiciousAccountReports.AddAsync(suspiciousAccountReport);

                await _db.SaveChangesAsync(cancellationToken);
            }

            private async Task<SuspiciousTweet> CreateNewSuspiciousTweet(ReportSuspiciousActivity request, Reporter reporter, SuspiciousAccount suspiciousAccount, CancellationToken cancellationToken)
            {
                SuspiciousTweet suspiciousTweet = new SuspiciousTweet()
                {
                    ReporterId = reporter.Id,
                    SuspiciousAccountId = suspiciousAccount.Id,
                    TweetId = request.SuspiciousTweetId,
                    TweetContent = request.SuspiciousTweetContent
                };
                await _db.SuspiciousTweets.AddAsync(suspiciousTweet);

                await _db.SaveChangesAsync(cancellationToken);
                return suspiciousTweet;
            }

            private async Task<SuspiciousAccount> CreateNewSuspiciousAccount(ReportSuspiciousActivity request, CancellationToken cancellationToken)
            {
                SuspiciousAccount suspiciousAccount = new SuspiciousAccount()
                {
                    TwitterUserId = request.SuspiciousAccountTwitterUserId,
                    TwitterScreenName = request.SuspiciousAccountTwitterScreenName,
                    TwitterJoinDate = request.SuspiciousAccountTwitterUserJoinDate
                };

                await _db.SuspiciousAccounts.AddAsync(suspiciousAccount);

                await _db.SaveChangesAsync(cancellationToken);

                return suspiciousAccount;
            }

            private async Task<Reporter> CreateNewReporter(ReportSuspiciousActivity request, CancellationToken cancellationToken)
            {
                Reporter reporter = new Reporter()
                {
                    TwitterUserId = request.ReporterTwitterUserId,
                    TwitterScreenName = request.ReporterTwitterScreenName
                };
                await _db.Reporters.AddAsync(reporter);

                await _db.SaveChangesAsync(cancellationToken);

                return reporter;
            }
        }
    }
}

