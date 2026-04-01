using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.Extensions;
using TravelingApp.Application.Features.Destinations.Models;
using TravelingApp.Domain.Entities;

namespace TravelingApp.Application.Features.Destinations.Queries.List
{
    public class ListDestinationsQueryHandler(IAppDbContext dbContext, IMapper mapper)
        : IRequestHandler<ListDestinationsQuery, DestinationResponse>
    {
        public async Task<DestinationResponse> Handle(ListDestinationsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Destination> query = dbContext.Destinations.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                var category = request.Category.ToLower();
                query = query.Where(d => d.Category.ToLower() == category);
            }

            var total = await query.CountAsync(cancellationToken);

            var destinations = await query
                .Page(request.PageSize, request.PageIndex, request.OrderBy!, request.OrderByAsc)
                .ToListAsync(cancellationToken);

            var dtos = mapper.Map<List<DestinationDto>>(destinations);

            return new DestinationResponse
            {
                Data = dtos,
                Count = total
            };
        }
    }
}
