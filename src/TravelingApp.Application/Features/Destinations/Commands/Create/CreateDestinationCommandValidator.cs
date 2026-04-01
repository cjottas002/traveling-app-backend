using FluentValidation;

namespace TravelingApp.Application.Features.Destinations.Commands.Create
{
    public class CreateDestinationCommandValidator : AbstractValidator<CreateDestinationCommand>
    {
        private static readonly string[] ValidCategories = ["beach", "mountain", "city", "nature"];

        public CreateDestinationCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(200);

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("El país es obligatorio")
                .MaximumLength(100);

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("La categoría es obligatoria")
                .Must(c => ValidCategories.Contains(c.ToLower()))
                .WithMessage("Categoría debe ser: beach, mountain, city o nature");

            RuleFor(x => x.Description)
                .MaximumLength(2000);

            RuleFor(x => x.ImageUrl)
                .MaximumLength(500);
        }
    }
}
