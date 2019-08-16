using RostamBot.Application.Features.SuspiciousActivity.Models;
using System.Collections.Generic;

namespace RostamBot.Application.Interfaces
{
    public interface IRostamBotService
    {
        //ToDo: remove overhead for SuspiciousAccount transmission
        Dictionary<SuspiciousAccountDto, bool> BlockUsers(string userAccessToken, string userAccessSecret, List<SuspiciousAccountDto> suspiciousAccountsToBlock, bool shouldBlock);
    }
}
