using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.Features.Destinations.Models;
using TravelingApp.Application.Models;

namespace TravelingApp.Application.Features.Destinations.Commands.Update
{
    public class UpdateDestinationCommandHandler(IAppDbContext dbContext, IMapper mapper)
        : IRequestHandler<UpdateDestinationCommand, FrameworkResponse<DestinationDto>>
    {
        public async Task<FrameworkResponse<DestinationDto>> Handle(UpdateDestinationCommand request, CancellationToken cancellationToken)
        {
            var destination = await dbContext.Destinations
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (destination == null)
            {
                return new FrameworkResponse<DestinationDto>
                {
                    Errors = [new ValidationResult("Destino no encontrado", [nameof(request.Id)])]
                };
            }

            destination.Name = request.Name;
            destination.Description = request.Description;
            destination.Country = request.Country;
            destination.ImageUrl = request.ImageUrl;
            destination.Category = request.Category;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new FrameworkResponse<DestinationDto>
            {
                Data = mapper.Map<DestinationDto>(destination),
                Count = 1
            };
        }
    }
}
