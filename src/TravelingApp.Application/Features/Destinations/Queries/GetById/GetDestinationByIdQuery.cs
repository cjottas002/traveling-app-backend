using MediatR;
using TravelingApp.Application.Features.Destinations.Models;
using TravelingApp.Application.Models;

namespace TravelingApp.Application.Features.Destinations.Queries.GetById
{
    public class GetDestinationByIdQuery : IRequest<FrameworkResponse<DestinationDto>>
    {
        public Guid Id { get; set; }
    }
}
