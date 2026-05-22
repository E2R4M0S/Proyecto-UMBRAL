using Moq;
using Umbral.Application.DTOs.Missions;
using Umbral.Application.Queries.Missions;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Primitives;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Tests.UnitTests;

public class GetMissionsQueryHandlerTests
{
    private readonly Mock<IMissionRepository> _repositoryMock;
    private readonly GetMissionsQueryHandler _handler;

    public GetMissionsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IMissionRepository>();
        _handler = new GetMissionsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsCorrectPageSize()
    {
        // Arrange
        var missions = CreateMissionList(25);
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, null, null, null))
            .ReturnsAsync(new PaginatedResult<Mission>(missions.Take(10).ToList(), 1, 10, 25));

        var query = new GetMissionsQuery();

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
    public async Task Handle_WithFilter_DelegatesFiltersToRepository()
    {
        // Arrange
        var missions = CreateMissionList(5);
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, "Facil", null, null))
            .ReturnsAsync(new PaginatedResult<Mission>(missions.Take(3).ToList(), 1, 10, 3));

        var query = new GetMissionsQuery(Difficulty: "Facil");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Items.Count);
        _repositoryMock.Verify(r => r.GetAllAsync(1, 10, "Facil", null, null), Times.Once);
    }

    [Fact]
    public async Task Handle_WithMultipleFilters_PassesAllToRepository()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 5, "Dificil", "Activa", "rescate"))
            .ReturnsAsync(new PaginatedResult<Mission>(new List<Mission>(), 1, 5, 0));

        var query = new GetMissionsQuery(1, 5, "Dificil", "Activa", "rescate");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        _repositoryMock.Verify(r => r.GetAllAsync(1, 5, "Dificil", "Activa", "rescate"), Times.Once);
    }

    [Fact]
    public async Task Handle_MapsMissionToMissionListItem_Correctly()
    {
        // Arrange
        var mission = CreateMission(
            Guid.NewGuid(),
            "Rescate en montaña",
            MissionDifficulty.Facil,
            MissionStatus.Activa);

        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, null, null, null))
            .ReturnsAsync(new PaginatedResult<Mission>(new List<Mission> { mission }, 1, 10, 1));

        var query = new GetMissionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var item = Assert.Single(result.Items);
        Assert.Equal(mission.Id, item.Id);
        Assert.Equal("Rescate en montaña", item.Title);
        Assert.Equal("Facil", item.Difficulty);
        Assert.Equal("Activa", item.Status);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyResult()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, null, null, null))
            .ReturnsAsync(PaginatedResult<Mission>.Empty);

        var query = new GetMissionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    private static List<Mission> CreateMissionList(int count)
    {
        var missions = new List<Mission>();
        for (int i = 0; i < count; i++)
        {
            missions.Add(CreateMission(
                Guid.NewGuid(),
                $"Mission {i + 1}",
                (MissionDifficulty)(i % 3),
                i % 2 == 0 ? MissionStatus.Borrador : MissionStatus.Activa));
        }
        return missions;
    }

    private static Mission CreateMission(Guid id, string title, MissionDifficulty difficulty, MissionStatus status)
    {
        var mission = new Mission(id, MissionTitle.Create(title), $"Description for {title}", difficulty, 30);

        // Use reflection to set Status since it has a private setter
        var statusField = typeof(Mission).GetProperty("Status")!;
        statusField.SetValue(mission, status);

        return mission;
    }
}
