using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using TravelingApp.Application.Configuration;
using TravelingApp.Application.Features.Account.Commands.Login;
using TravelingApp.Domain.Entities;

namespace TravelingApp.UnitTest.Application.Features.Account.Commands
{
    [TestFixture]
    public class LoginCommandHandlerTests
    {
        private Mock<UserManager<User>> _userManagerMock = null!;
        private Mock<SignInManager<User>> _signInManagerMock = null!;
        private LoginCommandHandler _handler = null!;

        [SetUp]
        public void Setup()
        {
            var userStore = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                null!, null!, null!, null!);

            var jwtOptions = Options.Create(new JwtOptions
            {
                Key = "super-secret-key-at-least-32-chars-long!",
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            });

            _handler = new LoginCommandHandler(_userManagerMock.Object, _signInManagerMock.Object, jwtOptions);
        }

        [Test]
        public async Task Handle_Should_Return_Valid_Response_When_Login_Succeeds()
        {
            var command = new LoginCommand { Username = "admin@test.com", Password = "Admin123!" };
            var user = new User { Id = "123", UserName = "admin@test.com", Email = "admin@test.com" };

            _userManagerMock.Setup(m => m.FindByNameAsync(command.Username!)).ReturnsAsync(user);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, command.Password!, false))
                .ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync([]);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data?.Token.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task Handle_Should_Return_Error_When_User_Not_Found()
        {
            var command = new LoginCommand { Username = "notfound@test.com", Password = "Admin123!" };

            _userManagerMock.Setup(m => m.FindByNameAsync(command.Username!)).ReturnsAsync((User?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.Errors.Should().ContainSingle();
        }

        [Test]
        public async Task Handle_Should_Return_Error_When_Invalid_Password()
        {
            var command = new LoginCommand { Username = "admin@test.com", Password = "wrongpass" };
            var user = new User { Id = "123", UserName = "admin@test.com", Email = "admin@test.com" };

            _userManagerMock.Setup(m => m.FindByNameAsync(command.Username!)).ReturnsAsync(user);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, command.Password!, false))
                .ReturnsAsync(SignInResult.Failed);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors!.First().MemberNames.Should().Contain("Password");
        }

        [Test]
        public async Task Handle_Should_Throw_When_UserManager_Fails()
        {
            var command = new LoginCommand { Username = "admin@test.com", Password = "Admin123!" };

            _userManagerMock.Setup(m => m.FindByNameAsync(command.Username!))
                .ThrowsAsync(new Exception("Unexpected error"));

            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowExactlyAsync<Exception>();
        }
    }
}
