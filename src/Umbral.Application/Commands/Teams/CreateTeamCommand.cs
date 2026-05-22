using MediatR;
using Umbral.Application.DTOs.Teams;

namespace Umbral.Application.Commands.Teams;

public record CreateTeamCommand(string Name, string? Description, Guid LeaderId, List<Guid>? MemberIds)
    : IRequest<CreateTeamResponse>;
