using MediatR;
using Umbral.Application.DTOs.Auth;

namespace Umbral.Application.Commands.Auth;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
