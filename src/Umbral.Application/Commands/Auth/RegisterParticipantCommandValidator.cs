using System.Text.RegularExpressions;
using FluentValidation;

namespace Umbral.Application.Commands.Auth;

public class RegisterParticipantCommandValidator : AbstractValidator<RegisterParticipantCommand>
{
    public RegisterParticipantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(1, 100).WithMessage("Name must be between 1 and 100 characters.");

        RuleFor(x => x.Alias)
            .NotEmpty().WithMessage("Alias is required.")
            .Length(3, 20).WithMessage("Alias must be between 3 and 20 characters.")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Alias must contain only letters, numbers, and underscores.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");
    }
}
