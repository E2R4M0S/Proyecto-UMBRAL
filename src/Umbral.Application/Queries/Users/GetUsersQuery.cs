using MediatR;
using Umbral.Application.DTOs.Users;
using Umbral.Domain.Primitives;

namespace Umbral.Application.Queries.Users;

public record GetUsersQuery(
    int Page = 1,
    int PageSize = 10,
    string? Role = null,
    string? Status = null,
    string? Search = null)
    : IRequest<PaginatedResult<UserListItem>>;
