using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.Features.Destinations.Models;
using TravelingApp.Application.Models;
using TravelingApp.Domain.Entities;

namespace TravelingApp.Application.Features.Destinations.Commands.Create
{
    public class CreateDestinationCommandHandler(
        IAppDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
        : IRequestHandler<CreateDestinationCommand, FrameworkResponse<DestinationDto>>
    {
        public async Task<FrameworkResponse<DestinationDto>> Handle(CreateDestinationCommand request, CancellationToken cancellationToken)
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var destination = new Destination
            {
                Name = request.Name,
                Description = request.Description,
                Country = request.Country,
                ImageUrl = request.ImageUrl,
                Category = request.Category,
                CreatedBy = userId
            };

            dbContext.Destinations.Add(destination);
            await dbContext.SaveChangesAsync(cancellationToken);

            var dto = mapper.Map<DestinationDto>(destination);

            return new FrameworkResponse<DestinationDto>
            {
                Data = dto,
                Count = 1
            };
        }
    }
}
