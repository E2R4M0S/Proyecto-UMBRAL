using MediatR;
using Umbral.Application.DTOs.Participants;

namespace Umbral.Application.Commands.Participants;

public record UpdateParticipantProfileCommand(
    Guid UserId,
    string? Name,
    string? Alias,
    string? CurrentPassword,
    string? NewPassword
) : IRequest<UpdateParticipantProfileResponse>;
