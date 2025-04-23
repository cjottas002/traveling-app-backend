using System.Text.Json.Serialization;
using TravelingApp.Application.Models;

namespace TravelingApp.Application.Features.Account.Models
{
    public class LoginDto : ResponseDto
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }
}
