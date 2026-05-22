using MediatR;
using Umbral.Application.DTOs.Missions;

namespace Umbral.Application.Queries.Missions;

public record GetMissionByIdQuery(Guid Id) : IRequest<MissionDetail>;
