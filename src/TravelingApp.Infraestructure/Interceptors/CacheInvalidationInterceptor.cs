using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TravelingApp.Application.Abstractions;

namespace TravelingApp.Infraestructure.Interceptors
{
    public class CacheInvalidationInterceptor(ICacheService cacheService) : SaveChangesInterceptor
    {
        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is null)
                return result;

            var changedEntityTypes = eventData.Context.ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .Select(e => e.Entity.GetType().Name)
                .Distinct();

            foreach (var entityName in changedEntityTypes)
            {
                var prefix = $"{entityName}s:";
                await cacheService.RemoveByPrefixAsync(prefix);
            }

            return result;
        }
    }
}
