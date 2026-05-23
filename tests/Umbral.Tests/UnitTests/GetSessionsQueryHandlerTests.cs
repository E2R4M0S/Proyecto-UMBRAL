using Moq;
using Umbral.Application.DTOs.Sessions;
using Umbral.Application.Queries.Sessions;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Primitives;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Tests.UnitTests;

public class GetSessionsQueryHandlerTests
{
    private readonly Mock<ISessionRepository> _repositoryMock;
    private readonly GetSessionsQueryHandler _handler;

    public GetSessionsQueryHandlerTests()
    {
        _repositoryMock = new Mock<ISessionRepository>();
        _handler = new GetSessionsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DefaultPagination_ReturnsCorrectPageSize()
    {
        // Arrange
        var sessions = CreateSessionList(25);
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<GetSessionsFilter>(), 1, 10))
            .ReturnsAsync(new PaginatedResult<Session>(sessions.Take(10).ToList(), 1, 10, 25));

        var query = new GetSessionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public async Task Handle_FilterByMissionId_FiltersCorrectly()
    {
        // Arrange
        var targetMissionId = Guid.NewGuid();
        var sessions = CreateSessionList(3);

        _repositoryMock
            .Setup(r => r.GetAllAsync(
                It.Is<GetSessionsFilter>(f => f.MissionId == targetMissionId), 1, 10))
            .ReturnsAsync(new PaginatedResult<Session>(sessions, 1, 10, 3));

        var query = new GetSessionsQuery(MissionId: targetMissionId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Items.Count);
        _repositoryMock.Verify(
            r => r.GetAllAsync(
                It.Is<GetSessionsFilter>(f => f.MissionId == targetMissionId), 1, 10),
            Times.Once);
    }

    [Fact]
    public async Task Handle_FilterByStatus_FiltersCorrectly()
    {
        // Arrange
        var sessions = CreateSessionList(5);

        _repositoryMock
            .Setup(r => r.GetAllAsync(
                It.Is<GetSessionsFilter>(f => f.Status == "Activa"), 1, 10))
            .ReturnsAsync(new PaginatedResult<Session>(sessions.Take(3).ToList(), 1, 10, 3));

        var query = new GetSessionsQuery(Status: "Activa");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Items.Count);
        _repositoryMock.Verify(
            r => r.GetAllAsync(
                It.Is<GetSessionsFilter>(f => f.Status == "Activa"), 1, 10),
            Times.Once);
    }

    [Fact]
    public async Task Handle_FilterByDateRange_FiltersCorrectly()
    {
        // Arrange
        var fromDate = new DateTime(2026, 5, 1);
        var toDate = new DateTime(2026, 5, 15);
        var sessions = CreateSessionList(2);

        _repositoryMock
            .Setup(r => r.GetAllAsync(
                It.Is<GetSessionsFilter>(f => f.FromDate == fromDate && f.ToDate == toDate), 1, 10))
            .ReturnsAsync(new PaginatedResult<Session>(sessions, 1, 10, 2));

        var query = new GetSessionsQuery(FromDate: fromDate, ToDate: toDate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Items.Count);
        _repositoryMock.Verify(
            r => r.GetAllAsync(
                It.Is<GetSessionsFilter>(f => f.FromDate == fromDate && f.ToDate == toDate), 1, 10),
            Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyResult()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<GetSessionsFilter>(), 1, 10))
            .ReturnsAsync(PaginatedResult<Session>.Empty);

        var query = new GetSessionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public async Task Handle_TeamCount_MapsCorrectly()
    {
        // Arrange
        var session = CreateSessionWithTeams(Guid.NewGuid(), "Test Session", 3);
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<GetSessionsFilter>(), 1, 10))
            .ReturnsAsync(new PaginatedResult<Session>(new List<Session> { session }, 1, 10, 1));

        var query = new GetSessionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var item = Assert.Single(result.Items);
        Assert.Equal(3, item.TeamCount);
    }

    private static List<Session> CreateSessionList(int count)
    {
        var sessions = new List<Session>();
        for (int i = 0; i < count; i++)
        {
            sessions.Add(CreateSession(
                Guid.NewGuid(),
                $"Session {i + 1}",
                Guid.NewGuid(),
                i % 2 == 0 ? SessionStatus.Activa : SessionStatus.Finalizada));
        }
        return sessions;
    }

    private static Session CreateSession(Guid id, string name, Guid missionId, SessionStatus status)
    {
        var mission = new Mission(missionId, MissionTitle.Create($"Mission for {name}"), null, MissionDifficulty.Facil, 30, MissionType.Tesoro);
        SetPrivateField(mission, "Status", MissionStatus.Activa);

        var session = new Session(id, name, missionId, "1234");
        SetPrivateField(session, "Status", status);
        SetPrivateField(session, "Mission", mission);
        SetPrivateField(session, "StartedAt", DateTime.UtcNow.AddHours(-1));

        return session;
    }

    private static Session CreateSessionWithTeams(Guid id, string name, int teamCount)
    {
        var missionId = Guid.NewGuid();
        var mission = new Mission(missionId, MissionTitle.Create($"Mission for {name}"), null, MissionDifficulty.Facil, 30, MissionType.Tesoro);
        SetPrivateField(mission, "Status", MissionStatus.Activa);

        var session = new Session(id, name, missionId, "1234");
        SetPrivateField(session, "Status", SessionStatus.Activa);
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
