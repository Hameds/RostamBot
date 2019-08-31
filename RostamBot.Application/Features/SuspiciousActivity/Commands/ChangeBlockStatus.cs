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
    public class ChangeBlockStatus : IRequest
    {
        public Guid SuspiciousAccountId { get; set; }

        public bool BlockStatus { get; set; }

        public class Handler : IRequestHandler<ChangeBlockStatus, Unit>
        {
            private readonly IRostamBotDbContext _db;
            private readonly IMediator _mediator;


            public Handler(IRostamBotDbContext db, IMediator mediator)
            {
                _db = db;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(ChangeBlockStatus request, CancellationToken cancellationToken)
            {
                var suspiciousAccount = await _db.SuspiciousAccounts
                    .SingleOrDefaultAsync(c => c.Id == request.SuspiciousAccountId, cancellationToken);

                if (suspiciousAccount == null)
                {
                    throw new NotFoundException(nameof(SuspiciousAccount), request.SuspiciousAccountId);
                }

                suspiciousAccount.ShouldBlock = request.BlockStatus;

                await _db.SaveChangesAsync(cancellationToken);

                //ToDo: use AutoMapper
                var suspiciousAccountDto = new SuspiciousAccountDto()
                {
                    Id = suspiciousAccount.Id,
                    TwitterUserId = suspiciousAccount.TwitterUserId,
                    TwitterScreenName = suspiciousAccount.TwitterScreenName,
                    TwitterJoinDate = suspiciousAccount.TwitterJoinDate
                };

                await _mediator.Publish(
                    new ChangeBlockStatusSaved
                    {
                        SuspiciousAccountDto = suspiciousAccountDto,
                        BlockStatus = request.BlockStatus
                    },
                    cancellationToken);

                return Unit.Value;
            }


        }
    }
}
