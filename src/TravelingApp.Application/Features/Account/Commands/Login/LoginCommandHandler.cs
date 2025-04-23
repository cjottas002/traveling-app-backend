using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TravelingApp.Application.Models;
using TravelingApp.Application.Features.Account.Models;
using TravelingApp.Application.Configuration;
using TravelingApp.Domain.Entities;

namespace TravelingApp.Application.Features.Account.Commands.Login
{
    public class LoginCommandHandler(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IOptions<JwtOptions> jwtOptions)
        : IRequestHandler<LoginCommand, FrameworkResponse<LoginDto>>
    {
        private readonly JwtOptions _jwt = jwtOptions.Value;

        public async Task<FrameworkResponse<LoginDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByNameAsync(request.Username!);

            if (user == null)
            {
                return new FrameworkResponse<LoginDto>
                {
                    Errors = [new ValidationResult("Usuario/Contraseña invalida", [request.Username!])]
                };
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password!, false);

            if (!result.Succeeded)
            {
                return new FrameworkResponse<LoginDto>
                {
                    Errors = [new ValidationResult("Usuario/Contraseña invalida", [nameof(request.Password)])]
                };
            }

            var token = await GenerateJwtTokenAsync(user);

            return new FrameworkResponse<LoginDto>
            {
                Data = new LoginDto { Token = token },
                Count = 1
            };
        }

        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = user.Id,
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
                [JwtRegisteredClaimNames.Name] = user.UserName ?? "",
                [JwtRegisteredClaimNames.Email] = user.Email ?? "",
                [ClaimTypes.NameIdentifier] = user.Id
            };

            var roles = await userManager.GetRolesAsync(user);
            claims[ClaimTypes.Role] = roles.ToList();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key!));

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_jwt.ExpireMinutes),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                Claims = claims
            };

            var handler = new JsonWebTokenHandler();
            return handler.CreateToken(descriptor);
        }
    }
}
