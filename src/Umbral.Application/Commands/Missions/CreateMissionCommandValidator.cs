using FluentValidation;

namespace Umbral.Application.Commands.Missions;

public class CreateMissionCommandValidator : AbstractValidator<CreateMissionCommand>
{
    public CreateMissionCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150).WithMessage("Title must be at most 150 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must be at most 500 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Difficulty)
            .NotEmpty().WithMessage("Difficulty is required.")
            .Must(d => d is "Facil" or "Media" or "Dificil")
            .WithMessage("Difficulty must be 'Facil', 'Media', or 'Dificil'.");

        RuleFor(x => x.TimeMinutes)
            .Must(t => t is 13 or 30 or 60 or 90)
            .WithMessage("TimeMinutes must be 13, 30, 60, or 90.");
    }
}
