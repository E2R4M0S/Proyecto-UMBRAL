using Moq;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Missions;
using Umbral.Application.Queries.Missions;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Tests.UnitTests;

public class GetMissionByIdQueryHandlerTests
{
    private readonly Mock<IMissionRepository> _repositoryMock;
    private readonly GetMissionByIdQueryHandler _handler;

    public GetMissionByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IMissionRepository>();
        _handler = new GetMissionByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingMission_ReturnsMissionDetail()
    {
        // Arrange
        var missionId = Guid.NewGuid();
        var mission = new Mission(
            missionId,
            MissionTitle.Create("Rescate en montaña"),
            "Rescatar a un escalador herido",
            MissionDifficulty.Dificil,
            60);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(missionId))
            .ReturnsAsync(mission);

        var query = new GetMissionByIdQuery(missionId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(missionId, result.Id);
        Assert.Equal("Rescate en montaña", result.Title);
        Assert.Equal("Rescatar a un escalador herido", result.Description);
        Assert.Equal("Dificil", result.Difficulty);
        Assert.Equal(60, result.TimeMinutes);
        Assert.Equal("Borrador", result.Status);
    }

    [Fact]
    public async Task Handle_NonExistentMission_ThrowsNotFoundException()
    {
        // Arrange
        var missionId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(missionId))
            .ReturnsAsync((Mission?)null);

        var query = new GetMissionByIdQuery(missionId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));

        Assert.Contains(missionId.ToString(), exception.Message);
    }

    [Fact]
    public async Task Handle_MissionNotFound_RepositoryCalledWithCorrectId()
    {
        // Arrange
        var missionId = Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(missionId))
            .ReturnsAsync((Mission?)null);

        var query = new GetMissionByIdQuery(missionId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));

        _repositoryMock.Verify(r => r.GetByIdAsync(missionId), Times.Once);
    }
}
