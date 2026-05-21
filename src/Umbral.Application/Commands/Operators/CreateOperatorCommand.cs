using MediatR;
using Umbral.Application.DTOs.Operators;

namespace Umbral.Application.Commands.Operators;

public record CreateOperatorCommand(string Name, string Email, string Password) : IRequest<CreateOperatorResponse>;
