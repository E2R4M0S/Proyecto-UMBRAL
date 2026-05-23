using FluentValidation;

namespace Umbral.Application.Commands.Operators;

public class DeactivateOperatorCommandValidator : AbstractValidator<DeactivateOperatorCommand>
{
    public DeactivateOperatorCommandValidator()
    {
        RuleFor(x => x.OperatorId).NotEmpty();
    }
}
