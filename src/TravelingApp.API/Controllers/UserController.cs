using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelingApp.Application.Features.Users.Queries.ListUsers;

namespace TravelingApp.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class UserController(IMediator mediator) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ListUsersQuery request)
        {
            return Ok(await mediator.Send(request));
        }
    }
}
