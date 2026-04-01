using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using TravelingApp.Application.Features.Account.Commands.Register;
using TravelingApp.Domain.Entities;

namespace TravelingApp.UnitTest.Application.Features.Account.Commands
{
    [TestFixture]
    public class RegisterCommandHandlerTests
    {
        private Mock<UserManager<User>> _userManagerMock = null!;
        private RegisterCommandHandler _handler = null!;

        [SetUp]
        public void Setup()
        {
            var userStore = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _handler = new RegisterCommandHandler(_userManagerMock.Object);
        }

        [Test]
        public async Task Handle_Should_Return_Success_When_Registration_Succeeds()
        {
            var command = new RegisterCommand { Username = "newuser", Password = "Admin123!" };

            _userManagerMock.Setup(m => m.FindByNameAsync(command.Username!)).ReturnsAsync((User?)null);
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), command.Password!))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Success.Should().BeTrue();
            result.Data?.IsRegistered.Should().BeTrue();
            _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), "Customer"), Times.Once);
        }

        [Test]
        public async Task Handle_Should_Return_Error_When_Username_Already_Exists()
        {
            var command = new RegisterCommand { Username = "existinguser", Password = "Admin123!" };
            var existingUser = new User { UserName = "existinguser" };

            _userManagerMock.Setup(m => m.FindByNameAsync(command.Username!)).ReturnsAsync(existingUser);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors!.First().MemberNames.Should().Contain("Username");
        }

        [Test]
        public async Task Handle_Should_Return_Errors_When_Creation_Fails()
        {
            var command = new RegisterCommand { Username = "newuser", Password = "123" };

            _userManagerMock.Setup(m => m.FindByNameAsync(command.Username!)).ReturnsAsync((User?)null);
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), command.Password!))
                .ReturnsAsync(IdentityResult.Failed(
                    new IdentityError { Code = "Password", Description = "La contraseña es demasiado débil" },
                    new IdentityError { Code = "Password", Description = "Debe tener al menos 8 caracteres" }));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Success.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }

        [Test]
        public async Task Handle_Should_Throw_Exception_When_UserManager_Fails()
        {
            var command = new RegisterCommand { Username = "erroruser", Password = "Admin123!" };

            _userManagerMock.Setup(m => m.FindByNameAsync(command.Username!))
                .ThrowsAsync(new Exception("Unexpected error"));

            var act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowExactlyAsync<Exception>();
        }
    }
}
