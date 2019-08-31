using Araye.Code.Cqrs.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Interfaces;
using RostamBot.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace RostamBot.Application.Features.SuspiciousActivity.Commands
{
    public class AddSuspiciousAccount : IRequest
    {
        public string TwitterScreenName { get; set; }


        public class Handler : IRequestHandler<AddSuspiciousAccount, Unit>
        {
            private readonly IRostamBotDbContext _db;
            private readonly IRostamBotManagerService _rostamBotManagerService;
            private readonly IMediator _mediator;


            public Handler(IRostamBotDbContext db,
                IRostamBotManagerService rostamBotManagerService,
                IMediator mediator)
            {
                _db = db;
                _rostamBotManagerService = rostamBotManagerService;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(AddSuspiciousAccount request, CancellationToken cancellationToken)
            {

                var twitterUserInfo = _rostamBotManagerService.GetSuspiciousAccountInfo(request.TwitterScreenName);


                if (await _db.SuspiciousAccounts.AnyAsync(x => x.TwitterUserId == twitterUserInfo.TwitterUserId))
                {
                    throw new AppException($"{request.TwitterScreenName} is already in block list");
                }

                var newSuspiciousAccount = new SuspiciousAccount()
                {
                    ShouldBlock = true,
                    TwitterJoinDate = twitterUserInfo.TwitterJoinDate,
                    TwitterScreenName = twitterUserInfo.TwitterScreenName,
                    TwitterUserId = twitterUserInfo.TwitterUserId
                };

                await _db.SuspiciousAccounts.AddAsync(newSuspiciousAccount);

                await _db.SaveChangesAsync(cancellationToken);

                var suspiciousAccountDto = new SuspiciousAccountDto()
                {
                    Id = newSuspiciousAccount.Id,
                    TwitterUserId = newSuspiciousAccount.TwitterUserId,
                    TwitterScreenName = newSuspiciousAccount.TwitterScreenName,
                    TwitterJoinDate = newSuspiciousAccount.TwitterJoinDate
                };

                await _mediator.Publish(
                    new ChangeBlockStatusSaved
                    {
                        SuspiciousAccountDto = twitterUserInfo,
                        BlockStatus = true
                    },
                    cancellationToken);

                return Unit.Value;
            }


        }
    }
}
