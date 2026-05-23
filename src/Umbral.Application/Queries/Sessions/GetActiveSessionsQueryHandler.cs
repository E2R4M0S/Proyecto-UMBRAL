using MediatR;
using Umbral.Application.DTOs.Sessions;
using Umbral.Domain.Repositories;

namespace Umbral.Application.Queries.Sessions;

public class GetActiveSessionsQueryHandler : IRequestHandler<GetActiveSessionsQuery, List<SessionListItem>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetActiveSessionsQueryHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<List<SessionListItem>> Handle(
        GetActiveSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var sessions = await _sessionRepository.GetActiveAsync();

        return sessions
            .Select(s => new SessionListItem(
                s.Id,
                s.Name,
                s.Mission.Title.ToString(),
                s.Status.ToString(),
                s.Pin,
                s.StartedAt,
                s.SessionTeams.Count))
            .ToList();
    }
}
