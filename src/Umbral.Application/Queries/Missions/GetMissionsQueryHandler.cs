using MediatR;
using Umbral.Application.DTOs.Missions;
using Umbral.Domain.Primitives;
using Umbral.Domain.Repositories;

namespace Umbral.Application.Queries.Missions;

public class GetMissionsQueryHandler : IRequestHandler<GetMissionsQuery, PaginatedResult<MissionListItem>>
{
    private readonly IMissionRepository _missionRepository;

    public GetMissionsQueryHandler(IMissionRepository missionRepository)
    {
        _missionRepository = missionRepository;
    }

    public async Task<PaginatedResult<MissionListItem>> Handle(
        GetMissionsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _missionRepository.GetAllAsync(
            request.Page,
            request.PageSize,
            request.Difficulty,
            request.Status,
            request.Search);

        var items = result.Items
            .Select(m => new MissionListItem(
                m.Id,
                m.Title.ToString(),
                m.Difficulty.ToString(),
                m.Type.ToString(),
                m.Status.ToString()))
            .ToList();

        return new PaginatedResult<MissionListItem>(items, result.Page, result.PageSize, result.TotalCount);
    }
}
