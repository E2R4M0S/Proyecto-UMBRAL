using MediatR;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Missions;
using Umbral.Domain.Repositories;

namespace Umbral.Application.Queries.Missions;

public class GetMissionByIdQueryHandler : IRequestHandler<GetMissionByIdQuery, MissionDetail>
{
    private readonly IMissionRepository _missionRepository;

    public GetMissionByIdQueryHandler(IMissionRepository missionRepository)
    {
        _missionRepository = missionRepository;
    }

    public async Task<MissionDetail> Handle(
        GetMissionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var mission = await _missionRepository.GetByIdAsync(request.Id);

        if (mission is null)
            throw new NotFoundException($"Mission with ID '{request.Id}' not found.");

        return new MissionDetail(
            mission.Id,
            mission.Title.ToString(),
            mission.Description,
            mission.Difficulty.ToString(),
            mission.TimeMinutes,
            mission.Status.ToString(),
            mission.CreatedAt,
            mission.UpdatedAt);
    }
}
