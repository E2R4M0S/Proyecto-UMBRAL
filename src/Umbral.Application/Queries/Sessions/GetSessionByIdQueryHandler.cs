using MediatR;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Sessions;
using Umbral.Domain.Repositories;

namespace Umbral.Application.Queries.Sessions;

public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, SessionDetail>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionByIdQueryHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<SessionDetail> Handle(
        GetSessionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.Id);

        if (session is null)
            throw new NotFoundException($"Session with ID '{request.Id}' not found.");

        return new SessionDetail(
            session.Id,
            session.Name,
            session.MissionId,
            session.Mission.Title.ToString(),
            session.Mission.Type.ToString(),
            session.Status.ToString(),
            session.Pin,
            session.StartedAt,
            session.EndedAt,
            session.SessionTeams
                .Select(st => new SessionTeamDto(st.TeamId, st.Team.Name.ToString(), st.Score))
                .ToArray());
    }
}
