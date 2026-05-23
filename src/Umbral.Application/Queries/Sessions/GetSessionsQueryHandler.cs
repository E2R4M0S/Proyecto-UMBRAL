using MediatR;
using Umbral.Application.DTOs.Sessions;
using Umbral.Domain.Primitives;
using Umbral.Domain.Repositories;

namespace Umbral.Application.Queries.Sessions;

public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, PaginatedResult<SessionListItem>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionsQueryHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<PaginatedResult<SessionListItem>> Handle(
        GetSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var filter = new GetSessionsFilter(
            request.MissionId,
            request.Status,
            request.FromDate,
            request.ToDate);

        var result = await _sessionRepository.GetAllAsync(filter, request.Page, request.PageSize);

        var items = result.Items
            .Select(s => new SessionListItem(
                s.Id,
                s.Name,
                s.Mission.Title.ToString(),
                s.Status.ToString(),
                s.Pin,
                s.StartedAt,
                s.SessionTeams.Count))
            .ToList();

        return new PaginatedResult<SessionListItem>(items, result.Page, result.PageSize, result.TotalCount);
    }
}
