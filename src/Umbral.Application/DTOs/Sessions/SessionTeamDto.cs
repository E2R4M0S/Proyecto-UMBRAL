namespace Umbral.Application.DTOs.Sessions;

/// <summary>
/// Nested DTO for a team participating in a session detail view.
/// </summary>
public record SessionTeamDto(
    Guid TeamId,
    string TeamName,
    int Score);
