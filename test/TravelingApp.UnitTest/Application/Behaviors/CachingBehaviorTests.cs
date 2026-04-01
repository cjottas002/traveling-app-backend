using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.Behaviors;
using TravelingApp.Application.Features.Users.Models;
using TravelingApp.Application.Features.Users.Queries.ListUsers;
using TravelingApp.Application.Features.Account.Commands.Login;
using TravelingApp.Application.Features.Account.Models;
using TravelingApp.Application.Models;

namespace TravelingApp.UnitTest.Application.Behaviors
{
    [TestFixture]
    public class CachingBehaviorTests
    {
        private Mock<ICacheService> _cacheServiceMock = null!;

        [SetUp]
        public void Setup()
        {
            _cacheServiceMock = new Mock<ICacheService>();
        }

        [Test]
        public async Task Handle_Should_Skip_Cache_When_Request_Is_Not_Cacheable()
        {
            var behavior = new CachingBehavior<LoginCommand, FrameworkResponse<LoginDto>>(_cacheServiceMock.Object);
            var expected = new FrameworkResponse<LoginDto> { Count = 1 };

            var result = await behavior.Handle(
                new LoginCommand(),
                (ct) => Task.FromResult(expected),
                CancellationToken.None);

            result.Should().BeSameAs(expected);
            _cacheServiceMock.Verify(c => c.GetAsync<It.IsAnyType>(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Handle_Should_Return_Cached_Response_When_Cache_Hit()
        {
            var behavior = new CachingBehavior<ListUsersQuery, UserResponse>(_cacheServiceMock.Object);
            var cached = new UserResponse { Count = 5 };
            var query = new ListUsersQuery { PageIndex = 1, PageSize = 10 };

            _cacheServiceMock.Setup(c => c.GetAsync<UserResponse>(query.CacheKey))
                .ReturnsAsync(cached);

            var nextCalled = false;
            var result = await behavior.Handle(
                query,
                (ct) => { nextCalled = true; return Task.FromResult(new UserResponse()); },
                CancellationToken.None);

            result.Should().BeSameAs(cached);
            nextCalled.Should().BeFalse();
        }

        [Test]
        public async Task Handle_Should_Execute_And_Cache_When_Cache_Miss()
        {
            var behavior = new CachingBehavior<ListUsersQuery, UserResponse>(_cacheServiceMock.Object);
            var query = new ListUsersQuery { PageIndex = 1, PageSize = 10 };
            var expected = new UserResponse { Count = 3 };

            _cacheServiceMock.Setup(c => c.GetAsync<UserResponse>(query.CacheKey))
                .ReturnsAsync((UserResponse?)null);

            var result = await behavior.Handle(
                query,
                (ct) => Task.FromResult(expected),
                CancellationToken.None);

            result.Should().BeSameAs(expected);
            _cacheServiceMock.Verify(c => c.SetAsync(
                query.CacheKey,
                expected,
                query.SlidingExpirationMinutes,
                query.AbsoluteExpirationMinutes), Times.Once);
        }
    }
}
