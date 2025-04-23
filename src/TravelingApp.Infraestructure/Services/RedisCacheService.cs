using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.DependencyInjection;

namespace TravelingApp.Infraestructure.Services
{
    [ScopedService(typeof(ICacheService))]
    public class RedisCacheService(
        IDistributedCache cache,
        IConnectionMultiplexer redis,
        ILogger<RedisCacheService> logger) : ICacheService
    {
        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var data = await cache.GetStringAsync(key);
                if (string.IsNullOrEmpty(data))
                    return default;

                return JsonSerializer.Deserialize<T>(data);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Redis cache GET failed for key: {Key}", key);
                return default;
            }
        }

        public Task SetAsync<T>(string key, T value, double? slidingExpirationMinutes = null, double? absoluteExpirationRelativeToNowMinutes = null)
        {
            var slidingTs = slidingExpirationMinutes.HasValue
                ? TimeSpan.FromMinutes(slidingExpirationMinutes.Value)
                : (TimeSpan?)null;

            var absoluteTs = absoluteExpirationRelativeToNowMinutes.HasValue
                ? TimeSpan.FromMinutes(absoluteExpirationRelativeToNowMinutes.Value)
                : (TimeSpan?)null;

            return SetAsync(key, value, slidingTs, absoluteTs);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions();

                if (slidingExpiration.HasValue)
                    options.SetSlidingExpiration(slidingExpiration.Value);

                if (absoluteExpirationRelativeToNow.HasValue)
                    options.SetAbsoluteExpiration(absoluteExpirationRelativeToNow.Value);

                var data = JsonSerializer.Serialize(value);
                await cache.SetStringAsync(key, data, options);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Redis cache SET failed for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Redis cache REMOVE failed for key: {Key}", key);
            }
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            try
            {
                var server = redis.GetServers().First(s => !s.IsReplica);
                var db = redis.GetDatabase();
                var keys = server.Keys(pattern: $"*{prefix}*").ToArray();

                if (keys.Length > 0)
                    await db.KeyDeleteAsync(keys);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Redis cache REMOVE BY PREFIX failed for: {Prefix}", prefix);
            }
        }
    }
}
