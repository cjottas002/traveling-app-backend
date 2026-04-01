using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using TravelingApp.Application.Behaviors;
using TravelingApp.Application.Features.Account.Commands.Login;
using TravelingApp.Application.Features.Account.Models;
using TravelingApp.Application.Models;

namespace TravelingApp.UnitTest.Application.Behaviors
{
    [TestFixture]
    public class ValidationBehaviorTests
    {
        [Test]
        public async Task Handle_Should_Call_Next_When_No_Validators()
        {
            var validators = Enumerable.Empty<IValidator<LoginCommand>>();
            var behavior = new ValidationBehavior<LoginCommand, FrameworkResponse<LoginDto>>(validators);
            var expected = new FrameworkResponse<LoginDto> { Count = 1 };

            var result = await behavior.Handle(
                new LoginCommand(),
                (ct) => Task.FromResult(expected),
                CancellationToken.None);

            result.Should().BeSameAs(expected);
        }

        [Test]
        public async Task Handle_Should_Call_Next_When_Validation_Passes()
        {
            var validator = new Mock<IValidator<LoginCommand>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<LoginCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var behavior = new ValidationBehavior<LoginCommand, FrameworkResponse<LoginDto>>([validator.Object]);
            var expected = new FrameworkResponse<LoginDto> { Count = 1 };

            var result = await behavior.Handle(
                new LoginCommand(),
                (ct) => Task.FromResult(expected),
                CancellationToken.None);

            result.Should().BeSameAs(expected);
        }

        [Test]
        public async Task Handle_Should_Throw_ValidationException_When_Validation_Fails()
        {
            var validator = new Mock<IValidator<LoginCommand>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<LoginCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(
                [
                    new ValidationFailure("Username", "El nombre de usuario es obligatorio")
                ]));

            var behavior = new ValidationBehavior<LoginCommand, FrameworkResponse<LoginDto>>([validator.Object]);

            var act = async () => await behavior.Handle(
                new LoginCommand(),
                (ct) => Task.FromResult(new FrameworkResponse<LoginDto>()),
                CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Handle_Should_Aggregate_Errors_From_Multiple_Validators()
        {
            var validator1 = new Mock<IValidator<LoginCommand>>();
            validator1.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<LoginCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult([new ValidationFailure("Username", "Error 1")]));

            var validator2 = new Mock<IValidator<LoginCommand>>();
            validator2.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<LoginCommand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult([new ValidationFailure("Password", "Error 2")]));

            var behavior = new ValidationBehavior<LoginCommand, FrameworkResponse<LoginDto>>(
                [validator1.Object, validator2.Object]);

            try
            {
                await behavior.Handle(
                    new LoginCommand(),
                    (ct) => Task.FromResult(new FrameworkResponse<LoginDto>()),
                    CancellationToken.None);

                Assert.Fail("Should have thrown");
            }
            catch (ValidationException ex)
            {
                ex.Errors.Should().HaveCount(2);
            }
        }
    }
}
