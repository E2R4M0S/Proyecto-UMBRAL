using FluentAssertions;
using Moq;
using Umbral.Application.Commands.Teams;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Teams;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Tests.UnitTests;

public class CreateTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly CreateTeamCommandHandler _handler;

    public CreateTeamCommandHandlerTests()
    {
        _teamRepoMock = new Mock<ITeamRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _handler = new CreateTeamCommandHandler(_teamRepoMock.Object, _userRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequestWithNameAndLeaderOnly_ReturnsCreateTeamResponse()
    {
        // Arrange
        var leaderId = Guid.NewGuid();
        var leader = new User(leaderId, "Leader", Email.Create("leader@test.com"), "hash", UserRole.Participant);

        _teamRepoMock
            .Setup(r => r.ExistsByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(leaderId))
            .ReturnsAsync(leader);

        var command = new CreateTeamCommand("Alpha Team", null, leaderId, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TeamId.Should().NotBeEmpty();

        _teamRepoMock.Verify(r => r.AddAsync(It.Is<Team>(t =>
            t.Name.Value == "Alpha Team" &&
            t.Description == null &&
            t.LeaderId == leaderId &&
            t.Score == 0 &&
            t.Status == TeamStatus.Activo)), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequestWithMembers_ReturnsCreateTeamResponse()
    {
        // Arrange
        var leaderId = Guid.NewGuid();
        var member1Id = Guid.NewGuid();
        var member2Id = Guid.NewGuid();

        var leader = new User(leaderId, "Leader", Email.Create("leader@test.com"), "hash", UserRole.Participant);
        var member1 = new User(member1Id, "Member1", Email.Create("member1@test.com"), "hash", UserRole.Participant);
        var member2 = new User(member2Id, "Member2", Email.Create("member2@test.com"), "hash", UserRole.Participant);

        _teamRepoMock
            .Setup(r => r.ExistsByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(leaderId))
            .ReturnsAsync(leader);

        _userRepoMock
            .Setup(r => r.GetParticipantsByIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<User> { member1, member2 });

        var command = new CreateTeamCommand("Alpha Team", "A great team", leaderId, new List<Guid> { member1Id, member2Id });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TeamId.Should().NotBeEmpty();

        _teamRepoMock.Verify(r => r.AddAsync(It.Is<Team>(t =>
            t.Name.Value == "Alpha Team" &&
            t.Description == "A great team" &&
            t.Members.Count == 2)), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateName_ThrowsConflictException()
    {
        // Arrange
        _teamRepoMock
            .Setup(r => r.ExistsByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var command = new CreateTeamCommand("Alpha Team", null, Guid.NewGuid(), null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("A team with this name already exists.");

        _teamRepoMock.Verify(r => r.AddAsync(It.IsAny<Team>()), Times.Never);
    }

    [Fact]
    public async Task Handle_LeaderNotFound_ThrowsArgumentException()
    {
        // Arrange
        var leaderId = Guid.NewGuid();

        _teamRepoMock
            .Setup(r => r.ExistsByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(leaderId))
            .ReturnsAsync((User?)null);

        var command = new CreateTeamCommand("Alpha Team", null, leaderId, null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Leader not found.*");

        _teamRepoMock.Verify(r => r.AddAsync(It.IsAny<Team>()), Times.Never);
    }

    [Fact]
    public async Task Handle_LeaderNotParticipant_ThrowsArgumentException()
    {
        // Arrange
        var leaderId = Guid.NewGuid();
        var leader = new User(leaderId, "OperatorUser", Email.Create("op@test.com"), "hash", UserRole.Operator);

        _teamRepoMock
            .Setup(r => r.ExistsByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(leaderId))
            .ReturnsAsync(leader);

        var command = new CreateTeamCommand("Alpha Team", null, leaderId, null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Leader must have the Participant role.*");

        _teamRepoMock.Verify(r => r.AddAsync(It.IsAny<Team>()), Times.Never);
    }

    [Fact]
    public async Task Handle_MemberAlreadyInAnotherTeam_ThrowsArgumentException()
    {
        // Arrange
        var leaderId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        var leader = new User(leaderId, "Leader", Email.Create("leader@test.com"), "hash", UserRole.Participant);
        var member = new User(memberId, "Member", Email.Create("member@test.com"), "hash", UserRole.Participant);

        // Simulate member already in another team via reflection (private setter)
        typeof(User).GetProperty(nameof(User.TeamId))?.SetValue(member, Guid.NewGuid());

        _teamRepoMock
            .Setup(r => r.ExistsByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(leaderId))
            .ReturnsAsync(leader);

        _userRepoMock
            .Setup(r => r.GetParticipantsByIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<User> { member });

        var command = new CreateTeamCommand("Alpha Team", null, leaderId, new List<Guid> { memberId });

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Member {memberId} is already assigned to another team.*");

        _teamRepoMock.Verify(r => r.AddAsync(It.IsAny<Team>()), Times.Never);
    }
}
