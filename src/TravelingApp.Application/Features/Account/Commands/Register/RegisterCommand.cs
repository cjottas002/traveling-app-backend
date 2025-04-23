using System.Text.Json.Serialization;
using MediatR;
using TravelingApp.Application.Models;
using TravelingApp.Application.Features.Account.Models;

namespace TravelingApp.Application.Features.Account.Commands.Register
{
    public class RegisterCommand : IRequest<FrameworkResponse<RegisterDto>>
    {
        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}
