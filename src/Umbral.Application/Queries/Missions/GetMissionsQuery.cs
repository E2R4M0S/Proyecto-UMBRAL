using MediatR;
using Umbral.Domain.Primitives;
using Umbral.Application.DTOs.Missions;

namespace Umbral.Application.Queries.Missions;

public record GetMissionsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Difficulty = null,
    string? Status = null,
    string? Search = null)
    : IRequest<PaginatedResult<MissionListItem>>;
