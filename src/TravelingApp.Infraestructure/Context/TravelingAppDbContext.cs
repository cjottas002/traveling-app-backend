using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TravelingApp.Application.Abstractions;
using TravelingApp.Domain.Entities;

namespace TravelingApp.Infraestructure.Context
{
    public class TravelingAppDbContext(DbContextOptions<TravelingAppDbContext> options, IConfiguration configuration) : IdentityDbContext<User>(options), IAppDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
            var connectionString = configuration.GetConnectionString("TravelingAppDbConnection");
            optionsBuilder.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "TravelingApp");
            });
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("TravelingApp");

            builder.ApplyConfigurationsFromAssembly(typeof(TravelingAppDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity is User user)
                    user.UpdatedAt = DateTime.UtcNow;

                if (entry.Entity is Destination destination)
                    destination.UpdatedAt = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<Destination> Destinations { get; set; }
    }
}
