using Microsoft.EntityFrameworkCore;
using TravelingApp.Domain.Entities;

namespace TravelingApp.Application.Abstractions
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Destination> Destinations { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
