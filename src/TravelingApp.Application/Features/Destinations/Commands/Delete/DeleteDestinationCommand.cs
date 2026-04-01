using MediatR;
using TravelingApp.Application.Models;

namespace TravelingApp.Application.Features.Destinations.Commands.Delete
{
    public class DeleteDestinationCommand : IRequest<FrameworkResponse<ResponseDto>>
    {
        public Guid Id { get; set; }
    }
}
