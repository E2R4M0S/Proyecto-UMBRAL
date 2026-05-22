namespace Umbral.Application.DTOs.Missions;

/// <summary>
/// Lightweight DTO for mission catalog list view.
/// </summary>
public record MissionListItem(
    Guid Id,
    string Title,
    string Difficulty,
    string Type,
    string Status);
