using MediatR;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.Features.Destinations.Models;
using TravelingApp.Application.Models;

namespace TravelingApp.Application.Features.Destinations.Queries.List
{
    public class ListDestinationsQuery : FrameworkRequest, IRequest<DestinationResponse>, ICacheableQuery
    {
        public string? Category { get; set; }

        public string CacheKey => $"Destinations:page={PageIndex}:size={PageSize}:sort={OrderBy}:{(OrderByAsc ? "asc" : "desc")}:cat={Category ?? "all"}";
        public double? SlidingExpirationMinutes => 5;
        public double? AbsoluteExpirationMinutes => 10;
    }
}
