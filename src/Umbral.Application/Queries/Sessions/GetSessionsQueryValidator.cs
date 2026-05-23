using FluentValidation;
using Umbral.Domain.Enums;

namespace Umbral.Application.Queries.Sessions;

public class GetSessionsQueryValidator : AbstractValidator<GetSessionsQuery>
{
    public GetSessionsQueryValidator()
    {
        When(x => x.Status is not null, () =>
        {
            RuleFor(x => x.Status)
                .Must(status => Enum.TryParse<SessionStatus>(status, out _))
                .WithMessage("Invalid session status value.");
        });

        RuleFor(x => x.PageSize)
            .LessThanOrEqualTo(50)
            .WithMessage("Page size cannot exceed 50.");
    }
}
