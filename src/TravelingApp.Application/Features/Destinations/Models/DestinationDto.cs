using System.Text.Json.Serialization;
using TravelingApp.Application.Models;

namespace TravelingApp.Application.Features.Destinations.Models
{
    public class DestinationDto : ResponseDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = string.Empty;

        [JsonPropertyName("createdAt")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public long UpdatedAt { get; set; }
    }

    public class DestinationResponse : FrameworkResponse<DestinationDto>
    {
        [JsonPropertyName("data")]
        public new List<DestinationDto> Data { get; set; } = [];
    }
}
