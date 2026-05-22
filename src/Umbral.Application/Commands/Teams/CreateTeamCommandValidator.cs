using FluentValidation;

namespace Umbral.Application.Commands.Teams;

public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required.")
            .MaximumLength(100).WithMessage("Team name must be at most 100 characters.");

        RuleFor(x => x.LeaderId)
            .NotEmpty().WithMessage("Leader is required.");

        RuleForEach(x => x.MemberIds)
            .NotEmpty().WithMessage("Member ID cannot be empty.")
            .When(x => x.MemberIds is not null && x.MemberIds.Count > 0);
    }
}
