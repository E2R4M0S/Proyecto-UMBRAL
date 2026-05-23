namespace Umbral.Domain.Repositories;

/// <summary>
/// Filter parameters for querying sessions.
/// All properties are optional — null means "no filter".
/// </summary>
public record GetSessionsFilter(
    Guid? MissionId = null,
    string? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null);
