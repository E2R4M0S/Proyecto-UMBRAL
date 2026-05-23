using FluentValidation;

namespace Umbral.Application.Commands.Participants;

public class UpdateParticipantProfileCommandValidator
    : AbstractValidator<UpdateParticipantProfileCommand>
{
    public UpdateParticipantProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        When(x => x.Alias != null, () =>
        {
            RuleFor(x => x.Alias).MinimumLength(3).MaximumLength(20);
        });

        When(x => x.NewPassword != null, () =>
        {
            RuleFor(x => x.CurrentPassword).NotEmpty()
                .WithMessage("Current password is required when changing password.");
            RuleFor(x => x.NewPassword).MinimumLength(6);
        });
    }
}
