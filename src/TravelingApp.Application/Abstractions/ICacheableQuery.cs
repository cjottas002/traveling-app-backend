namespace TravelingApp.Application.Abstractions
{
    public interface ICacheableQuery
    {
        string CacheKey { get; }
        double? SlidingExpirationMinutes { get; }
        double? AbsoluteExpirationMinutes { get; }
    }
}
