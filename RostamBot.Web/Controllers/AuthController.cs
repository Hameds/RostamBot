using Araye.Code.Cqrs.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RostamBot.Application.Features.Users.Commands;
using System.Threading.Tasks;

namespace RostamBot.Web.Controllers
{
    [Route("api/v1/auth/[action]")]
    public class AuthController : BaseApiController
    {

        [AllowAnonymous]
        [HttpPost]
        [Produces("application/json")]
        [ActionName("login")]
        public async Task<IActionResult> Login([FromBody]LoginUser model)
        {
            return Ok(await Mediator.Send(model));
        }

    }
}
