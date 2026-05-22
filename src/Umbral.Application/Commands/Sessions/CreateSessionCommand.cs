using MediatR;
using Umbral.Application.DTOs.Sessions;

namespace Umbral.Application.Commands.Sessions;

public record CreateSessionCommand(string Name, Guid MissionId) : IRequest<CreateSessionResponse>;
