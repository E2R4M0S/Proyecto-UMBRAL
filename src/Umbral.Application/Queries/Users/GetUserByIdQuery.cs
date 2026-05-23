using MediatR;
using Umbral.Application.DTOs.Users;

namespace Umbral.Application.Queries.Users;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDetail>;
