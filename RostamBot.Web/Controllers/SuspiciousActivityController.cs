using Araye.Code.Cqrs.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RostamBot.Application.Features.SuspiciousActivity.Commands;
using RostamBot.Application.Features.SuspiciousActivity.Models;
using RostamBot.Application.Features.SuspiciousActivity.Queries;
using System.Threading.Tasks;

namespace RostamBot.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/twitter/[action]")]
    public class SuspiciousActivityController : BaseApiController
    {

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ActionName("add")]
        public async Task<IActionResult> AddSuspiciousTwitterAccount([FromBody]string twitterScreenName)
        {

            var addSuspiciousAccount = new AddSuspiciousAccount()
            {
                TwitterScreenName = twitterScreenName,
            };
            return Ok(await Mediator.Send(addSuspiciousAccount));
        }

        /// <summary>
        /// Get RostamBot's Twitter block list
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        [ActionName("list")]
        public async Task<ActionResult<BlockedAccountsListViewModel>> GetTwitterBlockList([FromQuery] GetTwitterBlockList model)
        {
            return Ok(await Mediator.Send(model));
        }


        /// <summary>
        /// Check if user is in RostamBot's Twitter block list or not
        /// </summary>
        /// <param name="twitterScreenName">ScreenName of twitter user</param>
        /// <returns>true if user is in RostamBot's Twitter block list and false if not</returns>
        [HttpGet("{twitterScreenName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        [ActionName("check")]
        public async Task<ActionResult<bool>> IsTwitterUserBlocked(string twitterScreenName)
        {
            return Ok(await Mediator.Send(new IsUserBlocked { TwitterScreenName = twitterScreenName }));
        }
    }
}
