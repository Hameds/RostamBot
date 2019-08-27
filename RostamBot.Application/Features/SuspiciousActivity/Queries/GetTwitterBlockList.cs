
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using X.PagedList;

namespace RostamBot.Application.Features.SuspiciousActivity.Queries
{
    public class GetTwitterBlockList : IRequest<BlockedAccountsListViewModel>
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 50;

        public class Handler : IRequestHandler<GetTwitterBlockList, BlockedAccountsListViewModel>
        {
            private readonly IRostamBotDbContext _db;
            private readonly IMapper _mapper;

            public Handler(IRostamBotDbContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<BlockedAccountsListViewModel> Handle(GetTwitterBlockList request, CancellationToken cancellationToken)
            {
                return new BlockedAccountsListViewModel
                {
                    BlockedAccounts = await _db.SuspiciousAccounts.Where(x => x.ShouldBlock.HasValue && x.ShouldBlock.Value)
                                        .ProjectTo<BlockedAccountDto>(_mapper.ConfigurationProvider)
                                        .ToPagedListAsync(request.Page, request.PageSize, cancellationToken)
                };
            }
        }
    }
}
