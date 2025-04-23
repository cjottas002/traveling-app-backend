namespace TravelingApp.Application.Configuration
{
    public class RedisOptions
    {
        public const string SectionName = "RedisOptions";

        public double? SlidingExpiration { get; set; }
        public double? AbsoluteExpirationRelativeToNow { get; set; }
    }
}
