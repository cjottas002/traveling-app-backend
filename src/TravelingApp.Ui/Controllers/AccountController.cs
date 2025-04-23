using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelingApp.Application.Features.Account.Commands.Login;
using TravelingApp.Application.Features.Account.Commands.Register;

namespace TravelingApp.Ui.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController(IMediator mediator) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        {
            var result = await mediator.Send(request);
            if (result.Errors != null && result.Errors.Any())
                return Unauthorized(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterCommand request)
        {
            var result = await mediator.Send(request);
            if (result.Errors != null && result.Errors.Any())
                return BadRequest(result);
            return Ok(result);
        }
    }
}
