using System.Text.Json.Serialization;
using TravelingApp.Application.Models;

namespace TravelingApp.Application.Features.Account.Models
{
    public class RegisterDto : ResponseDto
    {
        [JsonPropertyName("isRegistered")]
        public bool IsRegistered { get; set; }
    }
}
