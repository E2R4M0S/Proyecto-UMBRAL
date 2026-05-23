namespace Umbral.Application.DTOs.Sessions;

/// <summary>
/// Lightweight DTO for session list views (pagination and active list).
/// </summary>
public record SessionListItem(
    Guid Id,
    string Name,
    string MissionTitle,
    string Status,
    string Pin,
    DateTime? StartTime,
    int TeamCount);
