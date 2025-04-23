using System.Text.Json.Serialization;
using TravelingApp.Application.Models;

namespace TravelingApp.Application.Features.Users.Models
{
    public class UserResponse : FrameworkResponse<UserDto>
    {
        [JsonPropertyName("data")]
        public new List<UserDto> Data { get; set; } = [];
    }
}
