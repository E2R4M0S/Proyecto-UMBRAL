using MediatR;

namespace Umbral.Application.Commands.Operators;

public record DeactivateOperatorCommand(Guid OperatorId) : IRequest;
