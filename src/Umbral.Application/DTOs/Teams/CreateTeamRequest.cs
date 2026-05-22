namespace Umbral.Application.DTOs.Teams;

public record CreateTeamRequest(
    string Name,
    string? Description,
    Guid LeaderId,
    List<Guid>? MemberIds);
