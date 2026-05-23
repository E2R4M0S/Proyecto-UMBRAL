using MediatR;
using Umbral.Application.DTOs.Sessions;

namespace Umbral.Application.Queries.Sessions;

public record GetActiveSessionsQuery : IRequest<List<SessionListItem>>;
