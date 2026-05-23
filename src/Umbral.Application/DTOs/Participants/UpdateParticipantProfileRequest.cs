namespace Umbral.Application.DTOs.Participants;

public record UpdateParticipantProfileRequest(
    string? Name,
    string? Alias,
    string? CurrentPassword,
    string? NewPassword
);
