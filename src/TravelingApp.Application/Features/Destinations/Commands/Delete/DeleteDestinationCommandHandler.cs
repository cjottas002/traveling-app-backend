using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.Models;

namespace TravelingApp.Application.Features.Destinations.Commands.Delete
{
    public class DeleteDestinationCommandHandler(IAppDbContext dbContext)
        : IRequestHandler<DeleteDestinationCommand, FrameworkResponse<ResponseDto>>
    {
        public async Task<FrameworkResponse<ResponseDto>> Handle(DeleteDestinationCommand request, CancellationToken cancellationToken)
        {
            var destination = await dbContext.Destinations
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (destination == null)
            {
                return new FrameworkResponse<ResponseDto>
                {
                    Errors = [new ValidationResult("Destino no encontrado", [nameof(request.Id)])]
                };
            }

            dbContext.Destinations.Remove(destination);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new FrameworkResponse<ResponseDto>
            {
                Count = 1
            };
        }
    }
}
