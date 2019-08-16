using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Interfaces;
using RostamBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RostamBot.Application.Jobs
{
    public class SyncBlockListJob : ISyncBlockListJob
    {
        private readonly IRostamBotDbContext _db;
        private readonly IMediator _mediator;
        private readonly IRostamBotService _twitterBotService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SyncBlockListJob(IRostamBotDbContext db,
                                IMediator mediator,
                                IRostamBotService twitterBotService,
                                UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _mediator = mediator;
            _twitterBotService = twitterBotService;
            _userManager = userManager;
        }

        //ToDo: this is sync or async? only god knows! 
        public async Task ApplyBlockListForFirstTime(ApplicationUser rostamBotUser, CancellationToken cancellationToken)
        {

            var currentBlockedList = await _db.SuspiciousAccounts
                                                                .Where(x => x.ShouldBlock.HasValue && x.ShouldBlock.Value)
                                                                .Select(x => new SuspiciousAccountDto { Id = x.Id, TwitterUserId = x.TwitterUserId })
                                                                .ToListAsync();
            await ApplyBlockListAndLog(rostamBotUser, currentBlockedList, shouldBlock: true, cancellationToken);

        }

        private async Task ApplyBlockListAndLog(ApplicationUser rostamBotUser, List<SuspiciousAccountDto> suspiciousAccounts, bool shouldBlock, CancellationToken cancellationToken)
        {
            //ToDo:remove magic strings
            var userAccessToken = await _userManager.GetAuthenticationTokenAsync(rostamBotUser, "Twitter", "UserAccessToken");
            var userAccessSecret = await _userManager.GetAuthenticationTokenAsync(rostamBotUser, "Twitter", "UserAccessSecret");

            if (string.IsNullOrEmpty(userAccessToken) || string.IsNullOrEmpty(userAccessSecret))
                return;

            var blockListApplyResult = _twitterBotService.BlockUsers(userAccessToken, userAccessSecret, suspiciousAccounts, shouldBlock: true);

            foreach (var blockResult in blockListApplyResult)
            {
                SuspiciousAccountDto suspiciousAccount = blockResult.Key;

                var blockHistory = new SuspicousAccountBlockHistory()
                {
                    ActionDate = DateTime.Now,
                    IsBlocked = true,
                    SuspiciousAccountId = suspiciousAccount.Id,
                    UserId = rostamBotUser.Id
                };

                await _db.SuspicousAccountBlockHistories.AddAsync(blockHistory);
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateBlockList(SuspiciousAccountDto suspiciousAccount, bool shouldBlock, CancellationToken cancellationToken)
        {
            var rostamBotUsers = _userManager.Users.ToList();

            foreach (var rostamBotUser in rostamBotUsers)
            {
                await ApplyBlockListAndLog(rostamBotUser, new List<SuspiciousAccountDto> { suspiciousAccount }, shouldBlock, cancellationToken);
            }
        }
    }
}
