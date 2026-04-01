using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelingApp.Application.Features.Destinations.Commands.Create;
using TravelingApp.Application.Features.Destinations.Commands.Delete;
using TravelingApp.Application.Features.Destinations.Commands.Update;
using TravelingApp.Application.Features.Destinations.Queries.GetById;
using TravelingApp.Application.Features.Destinations.Queries.List;

namespace TravelingApp.Ui.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class DestinationController(IMediator mediator) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ListDestinationsQuery request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await mediator.Send(new GetDestinationByIdQuery { Id = id });
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateDestinationCommand request)
        {
            var result = await mediator.Send(request);
            return result.Success ? Created("", result) : BadRequest(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateDestinationCommand request)
        {
            var result = await mediator.Send(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await mediator.Send(new DeleteDestinationCommand { Id = id });
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
