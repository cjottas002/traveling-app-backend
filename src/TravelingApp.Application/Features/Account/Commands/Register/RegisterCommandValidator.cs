using FluentValidation;

namespace TravelingApp.Application.Features.Account.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator() 
        {
            RuleFor(x => x.Username)
                      .NotEmpty().WithMessage("El nombre de usuario es obligatorio");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria")
                .MinimumLength(5).WithMessage("La contraseña debe tener al menos 5 caracteres");
        }
    }
}
