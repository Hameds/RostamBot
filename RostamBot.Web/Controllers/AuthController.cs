using Araye.Code.Cqrs.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RostamBot.Application.Features.Users.Commands;
using System.Threading.Tasks;

namespace RostamBot.Web.Controllers
{
    public class AuthController : BaseApiController
    {

        [AllowAnonymous]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> Login([FromBody]LoginUser model)
        {
            return Ok(await Mediator.Send(model));
        }

    }
}
