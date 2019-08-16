using Araye.Code.Cqrs.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

            public Handler(IRostamBotDbContext db)
            {
                _db = db;
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

                return Unit.Value;
            }


        }
    }
}
