namespace Umbral.Application.DTOs.Participants;

public record UpdateParticipantProfileResponse(
    Guid UserId,
    string Name,
    string? Alias,
    string? Token,
    DateTime? ExpiresAt
);
