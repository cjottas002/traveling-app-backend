using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TravelingApp.Application.Abstractions;

namespace TravelingApp.Infraestructure.Interceptors
{
    public class CacheInvalidationInterceptor(ICacheService cacheService) : SaveChangesInterceptor
    {
        private readonly HashSet<string> _pendingInvalidations = [];

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
            {
                var changedTypes = eventData.Context.ChangeTracker.Entries()
                    .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                    .Select(e => e.Entity.GetType().Name)
                    .Distinct();

                _pendingInvalidations.Clear();
                foreach (var type in changedTypes)
                {
                    _pendingInvalidations.Add($"{type}s:");
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            foreach (var prefix in _pendingInvalidations)
            {
                await cacheService.RemoveByPrefixAsync(prefix);
            }

            _pendingInvalidations.Clear();

            return result;
        }
    }
}
