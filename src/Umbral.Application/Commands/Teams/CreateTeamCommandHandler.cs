using MediatR;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Teams;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Application.Commands.Teams;

public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, CreateTeamResponse>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;

    public CreateTeamCommandHandler(ITeamRepository teamRepository, IUserRepository userRepository)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
    }

    public async Task<CreateTeamResponse> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        // 1. Check name uniqueness
        var exists = await _teamRepository.ExistsByNameAsync(request.Name.Trim());
        if (exists)
            throw new ConflictException("A team with this name already exists.");

        // 2. Validate leader exists and is Participant
        var leader = await _userRepository.GetByIdAsync(request.LeaderId);
        if (leader is null)
            throw new ArgumentException("Leader not found.", nameof(request.LeaderId));

        if (leader.Role != UserRole.Participant)
            throw new ArgumentException("Leader must have the Participant role.", nameof(request.LeaderId));

        if (leader.TeamId is not null)
            throw new ArgumentException("Leader is already assigned to another team.", nameof(request.LeaderId));

        // 3. Validate members if provided
        var members = new List<User>();
        if (request.MemberIds is not null && request.MemberIds.Count > 0)
        {
            var participantMembers = await _userRepository.GetParticipantsByIdsAsync(request.MemberIds);

            if (participantMembers.Count != request.MemberIds.Count)
            {
                var foundIds = participantMembers.Select(m => m.Id).ToHashSet();
                var missingIds = request.MemberIds.Where(id => !foundIds.Contains(id)).ToList();
                var missingDescriptions = string.Join(", ", missingIds);
                throw new ArgumentException($"Members not found or not Participants: {missingDescriptions}", nameof(request.MemberIds));
            }

            // Check no member is already in another team
            var alreadyInTeam = participantMembers.FirstOrDefault(m => m.TeamId is not null);
            if (alreadyInTeam is not null)
                throw new ArgumentException($"Member {alreadyInTeam.Id} is already assigned to another team.", nameof(request.MemberIds));

            members = participantMembers;
        }

        // 4. Create team
        var name = TeamName.Create(request.Name);
        var team = new Team(Guid.NewGuid(), name, request.Description, request.LeaderId);

        // 5. Assign members to team
        foreach (var member in members)
        {
            member.AssignToTeam(team);
            team.Members.Add(member);
        }

        // 6. Persist (leader relationship is via LeaderId FK, members via TeamId FK)
        await _teamRepository.AddAsync(team);

        return new CreateTeamResponse(team.Id);
    }
}
