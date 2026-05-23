using Moq;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Sessions;
using Umbral.Application.Queries.Sessions;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Tests.UnitTests;

public class GetSessionByIdQueryHandlerTests
{
    private readonly Mock<ISessionRepository> _repositoryMock;
    private readonly GetSessionByIdQueryHandler _handler;

    public GetSessionByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<ISessionRepository>();
        _handler = new GetSessionByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_SessionExists_ReturnsSessionDetailWithTeams()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var missionId = Guid.NewGuid();
        var mission = new Mission(missionId, MissionTitle.Create("Rescate Alpha"), "Desc", MissionDifficulty.Dificil, 60, MissionType.Tesoro);
        SetPrivateField(mission, "Status", MissionStatus.Publicada);

        var session = new Session(sessionId, "Sesión Mañana", missionId, "9876");
        SetPrivateField(session, "Status", SessionStatus.Activa);
        SetPrivateField(session, "Mission", mission);
        SetPrivateField(session, "StartedAt", new DateTime(2026, 5, 20, 10, 0, 0, DateTimeKind.Utc));

        var team1Id = Guid.NewGuid();
        var team2Id = Guid.NewGuid();
        var team1 = new Team(team1Id, TeamName.Create("Alpha"), null, Guid.NewGuid());
        var team2 = new Team(team2Id, TeamName.Create("Bravo"), null, Guid.NewGuid());

        var st1 = new SessionTeam(sessionId, team1Id);
        SetPrivateField(st1, "Score", 100);
        SetPrivateField(st1, "Team", team1);

        var st2 = new SessionTeam(sessionId, team2Id);
        SetPrivateField(st2, "Score", 85);
        SetPrivateField(st2, "Team", team2);

        SetPrivateField(session, "SessionTeams", new List<SessionTeam> { st1, st2 });

        _repositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);

        var query = new GetSessionByIdQuery(sessionId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sessionId, result.Id);
        Assert.Equal("Sesión Mañana", result.Name);
        Assert.Equal(missionId, result.MissionId);
        Assert.Equal("Rescate Alpha", result.MissionTitle);
        Assert.Equal("Tesoro", result.MissionType);
        Assert.Equal("Activa", result.Status);
        Assert.Equal("9876", result.Pin);
        Assert.NotNull(result.StartTime);
        Assert.Null(result.EndTime);

        Assert.Equal(2, result.Teams.Length);

        Assert.Contains(result.Teams, t => t.TeamId == team1Id && t.TeamName == "Alpha" && t.Score == 100);
        Assert.Contains(result.Teams, t => t.TeamId == team2Id && t.TeamName == "Bravo" && t.Score == 85);
    }

    [Fact]
    public async Task Handle_SessionExistsNoTeams_ReturnsEmptyTeams()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var missionId = Guid.NewGuid();
        var mission = new Mission(missionId, MissionTitle.Create("Solo Mission"), null, MissionDifficulty.Facil, 30, MissionType.Tesoro);
        SetPrivateField(mission, "Status", MissionStatus.Publicada);

        var session = new Session(sessionId, "Empty Teams", missionId, "0000");
        SetPrivateField(session, "Status", SessionStatus.Programada);
        SetPrivateField(session, "Mission", mission);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);

        var query = new GetSessionByIdQuery(sessionId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Teams);
    }

    [Fact]
    public async Task Handle_SessionNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync((Session?)null);

        var query = new GetSessionByIdQuery(sessionId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));

        Assert.Contains(sessionId.ToString(), exception.Message);
    }

    private static void SetPrivateField<T>(object obj, string fieldName, T value)
    {
        var prop = obj.GetType().GetProperty(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (prop is not null)
        {
            prop.SetValue(obj, value);
            return;
        }

        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field?.SetValue(obj, value);
    }
}
