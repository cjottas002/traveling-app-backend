using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TravelingApp.Application.Constants;
using TravelingApp.Application.Models;
using TravelingApp.Application.Features.Account.Models;
using TravelingApp.Domain.Entities;

namespace TravelingApp.Application.Features.Account.Commands.Register
{
    public class RegisterCommandHandler(UserManager<User> userManager)
        : IRequestHandler<RegisterCommand, FrameworkResponse<RegisterDto>>
    {
        public async Task<FrameworkResponse<RegisterDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await userManager.FindByNameAsync(request.Username!);
            if (existingUser != null)
            {
                return new FrameworkResponse<RegisterDto>
                {
                    Errors = [new ValidationResult("El Usuario ya está registrado", [nameof(request.Username)])]
                };
            }

            var user = new User { UserName = request.Username, Email = $"{request.Username}@test.com" };
            var result = await userManager.CreateAsync(user, request.Password!);

            if (!result.Succeeded)
            {
                return new FrameworkResponse<RegisterDto>
                {
                    Errors = result.Errors.Select(e => new ValidationResult(e.Description, [e.Code]))
                };
            }

            await userManager.AddToRoleAsync(user, AppConstant.RolCustomer);

            return new FrameworkResponse<RegisterDto>
            {
                Data = new RegisterDto { IsRegistered = true },
                Count = 1
            };
        }
    }
}
