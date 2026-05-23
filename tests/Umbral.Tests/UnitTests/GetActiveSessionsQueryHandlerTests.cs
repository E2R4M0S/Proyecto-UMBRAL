using Moq;
using Umbral.Application.DTOs.Sessions;
using Umbral.Application.Queries.Sessions;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Tests.UnitTests;

public class GetActiveSessionsQueryHandlerTests
{
    private readonly Mock<ISessionRepository> _repositoryMock;
    private readonly GetActiveSessionsQueryHandler _handler;

    public GetActiveSessionsQueryHandlerTests()
    {
        _repositoryMock = new Mock<ISessionRepository>();
        _handler = new GetActiveSessionsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_HasActiveSessions_ReturnsOnlyActivaAndEnPreparacion()
    {
        // Arrange
        var sessions = new List<Session>
        {
            CreateSession(Guid.NewGuid(), "Active 1", SessionStatus.Activa, 2),
            CreateSession(Guid.NewGuid(), "Active 2", SessionStatus.Activa, 0),
            CreateSession(Guid.NewGuid(), "Preparing", SessionStatus.EnPreparacion, 1),
        };

        _repositoryMock
            .Setup(r => r.GetActiveAsync())
            .ReturnsAsync(sessions);

        var query = new GetActiveSessionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count);

        var active1 = result[0];
        Assert.Equal("Active 1", active1.Name);
        Assert.Equal("Activa", active1.Status);
        Assert.Equal(2, active1.TeamCount);

        var active2 = result[1];
        Assert.Equal("Active 2", active2.Name);
        Assert.Equal("Activa", active2.Status);
        Assert.Equal(0, active2.TeamCount);

        var preparing = result[2];
        Assert.Equal("Preparing", preparing.Name);
        Assert.Equal("EnPreparacion", preparing.Status);
        Assert.Equal(1, preparing.TeamCount);
    }

    [Fact]
    public async Task Handle_NoActiveSessions_ReturnsEmptyList()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetActiveAsync())
            .ReturnsAsync(new List<Session>());

        var query = new GetActiveSessionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_TeamCount_MappedCorrectly()
    {
        // Arrange
        var session = CreateSession(Guid.NewGuid(), "Team Count Test", SessionStatus.Activa, 5);

        _repositoryMock
            .Setup(r => r.GetActiveAsync())
            .ReturnsAsync(new List<Session> { session });

        var query = new GetActiveSessionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var item = Assert.Single(result);
        Assert.Equal(5, item.TeamCount);
    }

    private static Session CreateSession(Guid id, string name, SessionStatus status, int teamCount)
    {
        var missionId = Guid.NewGuid();
        var mission = new Mission(missionId, MissionTitle.Create($"Mission for {name}"), null, MissionDifficulty.Facil, 30, MissionType.Tesoro);
        SetPrivateField(mission, "Status", MissionStatus.Activa);

        var session = new Session(id, name, missionId, "1234");
        SetPrivateField(session, "Status", status);
        SetPrivateField(session, "Mission", mission);
        SetPrivateField(session, "StartedAt", DateTime.UtcNow.AddHours(-1));

        var teams = new List<SessionTeam>();
        for (int i = 0; i < teamCount; i++)
        {
            var teamId = Guid.NewGuid();
            var team = new Team(teamId, TeamName.Create($"Team {i + 1}"), null, Guid.NewGuid());
            var sessionTeam = new SessionTeam(session.Id, teamId);
            SetPrivateField(sessionTeam, "Team", team);
            teams.Add(sessionTeam);
        }
        SetPrivateField(session, "SessionTeams", teams);

        return session;
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
