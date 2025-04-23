using System.Text.Json.Serialization;
using TravelingApp.Application.Models;

namespace TravelingApp.Application.Features.Users.Models
{
    public class UserDto : ResponseDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("updatedAt")]
        public long UpdatedAt { get; set; }
    }
}
