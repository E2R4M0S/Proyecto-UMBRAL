using FluentValidation;

namespace Umbral.Application.Commands.Sessions;

public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.MissionId)
            .NotEmpty();
    }
}
