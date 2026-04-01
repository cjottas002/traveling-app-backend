using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.Features.Users.Queries.ListUsers;
using TravelingApp.Application.Mapping;
using TravelingApp.Domain.Entities;

namespace TravelingApp.UnitTest.Application.Features.Users.Queries
{
    [TestFixture]
    public class ListUsersQueryHandlerTests
    {
        private IMapper _mapper = null!;
        private TestDbContext _dbContext = null!;
        private ListUsersQueryHandler _handler = null!;

        [SetUp]
        public void Setup()
        {
            _mapper = new MapperConfiguration(
                cfg => cfg.AddProfile(new UserProfile()),
                new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory()).CreateMapper();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TestDbContext(options);
            _handler = new ListUsersQueryHandler(_dbContext, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task Handle_Should_Return_Users_With_Count()
        {
            _dbContext.Users.AddRange(
                new User { Id = "1", UserName = "user1", Email = "user1@test.com", UpdatedAt = DateTime.UtcNow },
                new User { Id = "2", UserName = "user2", Email = "user2@test.com", UpdatedAt = DateTime.UtcNow });
            await _dbContext.SaveChangesAsync();

            var query = new ListUsersQuery { PageIndex = 1, PageSize = 10, OrderBy = "Id", OrderByAsc = true };

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Count.Should().Be(2);
            result.Data.Should().HaveCount(2);
            result.Data[0].Username.Should().Be("user1");
            result.Data[1].Username.Should().Be("user2");
        }

        [Test]
        public async Task Handle_Should_Return_Empty_When_No_Users()
        {
            var query = new ListUsersQuery { PageIndex = 1, PageSize = 10, OrderBy = "Id", OrderByAsc = true };

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Count.Should().Be(0);
            result.Data.Should().BeEmpty();
        }

        [Test]
        public async Task Handle_Should_Map_UpdatedAt_To_Unix_Milliseconds()
        {
            var timestamp = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            _dbContext.Users.Add(
                new User { Id = "1", UserName = "user1", Email = "user1@test.com", UpdatedAt = timestamp });
            await _dbContext.SaveChangesAsync();

            var query = new ListUsersQuery { PageIndex = 1, PageSize = 10, OrderBy = "Id", OrderByAsc = true };

            var result = await _handler.Handle(query, CancellationToken.None);

            var expectedMillis = new DateTimeOffset(timestamp).ToUnixTimeMilliseconds();
            result.Data[0].UpdatedAt.Should().Be(expectedMillis);
        }

        [Test]
        public async Task Handle_Should_Paginate_Results()
        {
            for (int i = 1; i <= 5; i++)
                _dbContext.Users.Add(new User { Id = i.ToString(), UserName = $"user{i}", Email = $"u{i}@test.com", UpdatedAt = DateTime.UtcNow });
            await _dbContext.SaveChangesAsync();

            var query = new ListUsersQuery { PageIndex = 1, PageSize = 2, OrderBy = "Id", OrderByAsc = true };

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Count.Should().Be(5);
            result.Data.Should().HaveCount(2);
        }
    }

    internal class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options), IAppDbContext
    {
        public DbSet<User> Users { get; set; }
    }
}
