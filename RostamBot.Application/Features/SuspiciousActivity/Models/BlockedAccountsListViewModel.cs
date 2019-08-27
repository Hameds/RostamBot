using X.PagedList;

namespace RostamBot.Application.Features.SuspiciousActivity.Models
{
    public class BlockedAccountsListViewModel
    {
        public IPagedList<BlockedAccountDto> BlockedAccounts { get; set; }
    }
}
