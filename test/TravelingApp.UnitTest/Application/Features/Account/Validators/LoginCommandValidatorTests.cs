using FluentAssertions;
using NUnit.Framework;
using TravelingApp.Application.Features.Account.Commands.Login;

namespace TravelingApp.UnitTest.Application.Features.Account.Validators
{
    [TestFixture]
    public class LoginCommandValidatorTests
    {
        private LoginCommandValidator _validator = null!;

        [SetUp]
        public void Setup()
        {
            _validator = new LoginCommandValidator();
        }

        [Test]
        public async Task Should_Pass_When_Valid()
        {
            var command = new LoginCommand { Username = "admin", Password = "Admin123!" };

            var result = await _validator.ValidateAsync(command);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_Fail_When_Username_Is_Empty()
        {
            var command = new LoginCommand { Username = "", Password = "Admin123!" };

            var result = await _validator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Username");
        }

        [Test]
        public async Task Should_Fail_When_Password_Is_Empty()
        {
            var command = new LoginCommand { Username = "admin", Password = "" };

            var result = await _validator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }

        [Test]
        public async Task Should_Fail_When_Password_Is_Too_Short()
        {
            var command = new LoginCommand { Username = "admin", Password = "1234" };

            var result = await _validator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }

        [Test]
        public async Task Should_Fail_When_Both_Fields_Are_Empty()
        {
            var command = new LoginCommand { Username = "", Password = "" };

            var result = await _validator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
        }
    }
}
