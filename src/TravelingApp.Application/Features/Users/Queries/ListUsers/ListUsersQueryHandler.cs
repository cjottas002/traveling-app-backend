using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.Features.Users.Models;
using TravelingApp.Application.Extensions;
using TravelingApp.Domain.Entities;

namespace TravelingApp.Application.Features.Users.Queries.ListUsers
{
    public class ListUsersQueryHandler(IAppDbContext dbContext, IMapper mapper)
        : IRequestHandler<ListUsersQuery, UserResponse>
    {
        public async Task<UserResponse> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            IQueryable<User> query = dbContext.Users.AsNoTracking();

            var total = await query.CountAsync(cancellationToken);

            var users = await query
                .Page(request.PageSize, request.PageIndex, request.OrderBy!, request.OrderByAsc)
                .ToListAsync(cancellationToken);

            var dtos = mapper.Map<List<UserDto>>(users);

            return new UserResponse
            {
                Data = dtos,
                Count = total
            };
        }
    }
}
