using MediatR;
using Umbral.Application.DTOs.Sessions;
using Umbral.Domain.Primitives;

namespace Umbral.Application.Queries.Sessions;

public record GetSessionsQuery(
    int Page = 1,
    int PageSize = 10,
    Guid? MissionId = null,
    string? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null)
    : IRequest<PaginatedResult<SessionListItem>>;
