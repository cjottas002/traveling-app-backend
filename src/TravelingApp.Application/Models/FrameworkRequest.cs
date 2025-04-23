using System.Text.Json.Serialization;

namespace TravelingApp.Application.Models
{
    public class FrameworkRequest
    {
        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; } = 1;

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 10;

        [JsonPropertyName("orderBy")]
        public string? OrderBy { get; set; } = string.Empty;

        [JsonPropertyName("orderByAsc")]
        public bool OrderByAsc { get; set; } = true;
    }
}
