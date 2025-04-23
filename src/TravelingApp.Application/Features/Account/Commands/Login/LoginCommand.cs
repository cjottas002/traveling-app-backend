using System.Text.Json.Serialization;
using MediatR;
using TravelingApp.Application.Models;
using TravelingApp.Application.Features.Account.Models;

namespace TravelingApp.Application.Features.Account.Commands.Login
{
    public class LoginCommand : IRequest<FrameworkResponse<LoginDto>>
    {
        [JsonPropertyName("username")]
        public string? Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string? Password { get; set; } = string.Empty;
    }
}
