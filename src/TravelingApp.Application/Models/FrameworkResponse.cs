using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TravelingApp.Application.Models
{
    public interface IFrameworkResponse<T>
    {
        T? Data { get; set; }
        int Count { get; set; }
    }

    public class FrameworkResponse<TResponseDto> : IFrameworkResponse<TResponseDto> where TResponseDto : ResponseDto
    {
        [JsonPropertyName("data")]
        public TResponseDto? Data { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("errors")]
        public IEnumerable<ValidationResult>? Errors { get; set; } = [];

        [JsonPropertyName("success")]
        public bool Success => Errors == null || !Errors.Any();
    }
}
