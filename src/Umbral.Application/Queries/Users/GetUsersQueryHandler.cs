using MediatR;
using Umbral.Application.DTOs.Users;
using Umbral.Domain.Primitives;
using Umbral.Domain.Repositories;

namespace Umbral.Application.Queries.Users;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResult<UserListItem>>
{
    private readonly IUserRepository _repository;

    public GetUsersQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResult<UserListItem>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllAsync(
            request.Page,
            request.PageSize,
            request.Role,
            request.Status,
            request.Search);

        var items = result.Items
            .Select(u => new UserListItem(
                u.Id,
                u.Name,
                u.Email.ToString(),
                u.Role.ToString(),
                u.Status.ToString()))
            .ToList();

        return new PaginatedResult<UserListItem>(items, result.Page, result.PageSize, result.TotalCount);
    }
}
