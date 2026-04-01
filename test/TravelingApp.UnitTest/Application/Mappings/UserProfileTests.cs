using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TravelingApp.Application.Features.Users.Models;
using TravelingApp.Application.Mapping;
using TravelingApp.Domain.Entities;

namespace TravelingApp.UnitTest.Application.Mappings
{
    [TestFixture]
    public class UserProfileTests
    {
        private IMapper _mapper = null!;

        [SetUp]
        public void Setup()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new UserProfile()), new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory()).CreateMapper();
        }

        [Test]
        public void Configuration_Should_Be_Valid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new UserProfile()), new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
            config.AssertConfigurationIsValid();
        }

        [Test]
        public void Should_Map_User_To_UserDto()
        {
            var user = new User
            {
                Id = "abc",
                UserName = "testuser",
                Email = "test@test.com",
                UpdatedAt = new DateTime(2026, 6, 15, 10, 30, 0, DateTimeKind.Utc)
            };

            var dto = _mapper.Map<UserDto>(user);

            dto.Id.Should().Be("abc");
            dto.Username.Should().Be("testuser");
            dto.Email.Should().Be("test@test.com");
            dto.UpdatedAt.Should().Be(new DateTimeOffset(user.UpdatedAt).ToUnixTimeMilliseconds());
        }

        [Test]
        public void Should_Map_Null_UserName_To_Null_Username()
        {
            var user = new User { Id = "1", UserName = null, Email = null, UpdatedAt = DateTime.UtcNow };

            var dto = _mapper.Map<UserDto>(user);

            dto.Username.Should().BeNull();
            dto.Email.Should().BeNull();
        }
    }
}
