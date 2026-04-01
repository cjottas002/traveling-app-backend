using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.Features.Destinations.Models;
using TravelingApp.Application.Models;

namespace TravelingApp.Application.Features.Destinations.Queries.GetById
{
    public class GetDestinationByIdQueryHandler(IAppDbContext dbContext, IMapper mapper)
        : IRequestHandler<GetDestinationByIdQuery, FrameworkResponse<DestinationDto>>
    {
        public async Task<FrameworkResponse<DestinationDto>> Handle(GetDestinationByIdQuery request, CancellationToken cancellationToken)
        {
            var destination = await dbContext.Destinations
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (destination == null)
            {
                return new FrameworkResponse<DestinationDto>
                {
                    Errors = [new ValidationResult("Destino no encontrado", [nameof(request.Id)])]
                };
            }

            return new FrameworkResponse<DestinationDto>
            {
                Data = mapper.Map<DestinationDto>(destination),
                Count = 1
            };
        }
    }
}
