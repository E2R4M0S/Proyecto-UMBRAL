using FluentValidation;
using Umbral.Domain.Enums;

namespace Umbral.Application.Commands.Sessions;

public class UpdateSessionStatusCommandValidator : AbstractValidator<UpdateSessionStatusCommand>
{
    public UpdateSessionStatusCommandValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(status => Enum.TryParse<SessionStatus>(status, out _))
            .WithMessage("Invalid session status value.");
    }
}
