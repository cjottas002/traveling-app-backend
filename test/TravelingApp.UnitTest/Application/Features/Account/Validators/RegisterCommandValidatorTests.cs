using FluentAssertions;
using NUnit.Framework;
using TravelingApp.Application.Features.Account.Commands.Register;

namespace TravelingApp.UnitTest.Application.Features.Account.Validators
{
    [TestFixture]
    public class RegisterCommandValidatorTests
    {
        private RegisterCommandValidator _validator = null!;

        [SetUp]
        public void Setup()
        {
            _validator = new RegisterCommandValidator();
        }

        [Test]
        public async Task Should_Pass_When_Valid()
        {
            var command = new RegisterCommand { Username = "newuser", Password = "Admin123!" };

            var result = await _validator.ValidateAsync(command);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public async Task Should_Fail_When_Username_Is_Empty()
        {
            var command = new RegisterCommand { Username = "", Password = "Admin123!" };

            var result = await _validator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Username");
        }

        [Test]
        public async Task Should_Fail_When_Password_Is_Empty()
        {
            var command = new RegisterCommand { Username = "newuser", Password = "" };

            var result = await _validator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }

        [Test]
        public async Task Should_Fail_When_Password_Is_Too_Short()
        {
            var command = new RegisterCommand { Username = "newuser", Password = "1234" };

            var result = await _validator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }
    }
}
