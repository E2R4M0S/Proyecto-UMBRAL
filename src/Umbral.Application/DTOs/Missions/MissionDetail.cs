namespace Umbral.Application.DTOs.Missions;

/// <summary>
/// Full mission detail DTO for single-mission view.
/// </summary>
public record MissionDetail(
    Guid Id,
    string Title,
    string? Description,
    string Difficulty,
    int TimeMinutes,
    string Type,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
