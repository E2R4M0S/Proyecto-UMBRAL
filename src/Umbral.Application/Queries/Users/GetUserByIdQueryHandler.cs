using MediatR;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Users;
using Umbral.Domain.Repositories;

namespace Umbral.Application.Queries.Users;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDetail>
{
    private readonly IUserRepository _repository;

    public GetUserByIdQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDetail> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id);

        if (user is null)
            throw new NotFoundException($"User with ID '{request.Id}' not found.");

        return new UserDetail(
            user.Id,
            user.Name,
            user.Email.ToString(),
            user.Alias,
            user.Role.ToString(),
            user.Status.ToString(),
            user.TeamId,
            user.CreatedAt);
    }
}
