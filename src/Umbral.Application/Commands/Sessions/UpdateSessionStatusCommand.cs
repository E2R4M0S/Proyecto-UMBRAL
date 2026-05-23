using MediatR;

namespace Umbral.Application.Commands.Sessions;

public record UpdateSessionStatusCommand(Guid SessionId, string Status) : IRequest;
