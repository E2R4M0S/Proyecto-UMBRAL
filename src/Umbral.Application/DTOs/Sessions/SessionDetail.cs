namespace Umbral.Application.DTOs.Sessions;

/// <summary>
/// Full session detail DTO including mission info and participating teams.
/// </summary>
public record SessionDetail(
    Guid Id,
    string Name,
    Guid MissionId,
    string MissionTitle,
    string MissionType,
    string Status,
    string Pin,
    DateTime? StartTime,
    DateTime? EndTime,
    SessionTeamDto[] Teams);
