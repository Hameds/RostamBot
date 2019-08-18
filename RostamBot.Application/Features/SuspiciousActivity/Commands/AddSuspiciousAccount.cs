using Araye.Code.Cqrs.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Interfaces;
using RostamBot.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RostamBot.Application.Features.SuspiciousActivity.Commands
{
    public class AddSuspiciousAccount : IRequest
    {
        public string TwitterScreenName { get; set; }

        public long TwitterUserId { get; set; }

        public DateTime TwitterJoinDate { get; set; }

        public class Handler : IRequestHandler<AddSuspiciousAccount, Unit>
        {
            private readonly IRostamBotDbContext _db;
            private readonly IMediator _mediator;


            public Handler(IRostamBotDbContext db, IMediator mediator)
            {
                _db = db;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(AddSuspiciousAccount request, CancellationToken cancellationToken)
            {
                if (await _db.SuspiciousAccounts.AnyAsync(x => x.TwitterUserId == request.TwitterUserId))
                {
                    throw new AppException($"{request.TwitterScreenName} is already in block list");
                }

                var newSuspiciousAccount = new SuspiciousAccount()
                {
                    ShouldBlock = true,
                    TwitterJoinDate = request.TwitterJoinDate,
                    TwitterScreenName = request.TwitterScreenName,
                    TwitterUserId = request.TwitterUserId
                };

                await _db.SuspiciousAccounts.AddAsync(newSuspiciousAccount);

                await _db.SaveChangesAsync(cancellationToken);

                var suspiciousAccountDto = new SuspiciousAccountDto()
                {
                    Id = newSuspiciousAccount.Id,
                    TwitterUserId = newSuspiciousAccount.TwitterUserId
                };

                await _mediator.Publish(
                    new ChangeBlockStatusSaved
                    {
                        SuspiciousAccountDto = suspiciousAccountDto,
                        BlockStatus = true
                    },
                    cancellationToken);

                return Unit.Value;
            }


        }
    }
}
