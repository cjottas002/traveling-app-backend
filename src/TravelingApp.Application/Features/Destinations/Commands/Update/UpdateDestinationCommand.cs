using System.Text.Json.Serialization;
using MediatR;
using TravelingApp.Application.Features.Destinations.Models;
using TravelingApp.Application.Models;

namespace TravelingApp.Application.Features.Destinations.Commands.Update
{
    public class UpdateDestinationCommand : IRequest<FrameworkResponse<DestinationDto>>
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
    }
}
