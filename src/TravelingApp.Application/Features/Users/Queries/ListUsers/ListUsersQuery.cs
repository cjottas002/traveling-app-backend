using TravelingApp.Application.Models;
using MediatR;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.Features.Users.Models;

namespace TravelingApp.Application.Features.Users.Queries.ListUsers
{
    public class ListUsersQuery : FrameworkRequest, IRequest<UserResponse>, ICacheableQuery
    {
        public DateTime? LastSyncTime { get; set; } = DateTime.UtcNow;

        public string CacheKey => $"Users:page={PageIndex}:size={PageSize}:sort={OrderBy}:{(OrderByAsc ? "asc" : "desc")}";
        public double? SlidingExpirationMinutes => 5;
        public double? AbsoluteExpirationMinutes => 10;
    }
}
