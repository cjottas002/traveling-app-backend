using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TravelingApp.Infraestructure.Context;
using TravelingApp.Infraestructure.Interceptors;

namespace TravelingApp.Infraestructure
{
    public static class RegisterDbService
    {
        public static void RegisterDbServices(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnection = configuration.GetConnectionString("RedisDbConnection") ?? "localhost:6379";
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));

            services.AddScoped<CacheInvalidationInterceptor>();

            services.AddDbContext<TravelingAppDbContext>((sp, options) =>
            {
                var connectionString = configuration.GetConnectionString("TravelingAppDbConnection") ?? string.Empty;
                options.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "TravelingApp");
                });

                var interceptor = sp.GetRequiredService<CacheInvalidationInterceptor>();
                options.AddInterceptors(interceptor);
            });
        }
    }
}
