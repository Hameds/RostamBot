using MediatR;
using Microsoft.EntityFrameworkCore;
using RostamBot.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace RostamBot.Application.Features.SuspiciousActivity.Queries
{
    public class IsUserBlocked : IRequest<bool>
    {
        public string TwitterScreenName { get; set; }

        public class Handler : IRequestHandler<IsUserBlocked, bool>
        {
            private readonly IRostamBotDbContext _db;

            public Handler(IRostamBotDbContext db)
            {
                _db = db;
            }

            public async Task<bool> Handle(IsUserBlocked request, CancellationToken cancellationToken)
            {
                return await _db.SuspiciousAccounts.AnyAsync(x => x.TwitterScreenName == request.TwitterScreenName && x.ShouldBlock.HasValue && x.ShouldBlock.Value);
            }
        }
    }
}
