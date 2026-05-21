using MediatR;
using Umbral.Application.DTOs.Auth;

namespace Umbral.Application.Commands.Auth;

public record RegisterParticipantCommand(string Name, string Alias, string Email, string Password) : IRequest<LoginResponse>;
