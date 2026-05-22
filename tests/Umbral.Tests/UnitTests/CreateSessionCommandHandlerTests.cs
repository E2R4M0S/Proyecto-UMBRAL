using FluentAssertions;
using Moq;
using Umbral.Application.Commands.Sessions;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Sessions;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Tests.UnitTests;

public class CreateSessionCommandHandlerTests
{
    private readonly Mock<IMissionRepository> _missionRepoMock;
    private readonly Mock<ISessionRepository> _sessionRepoMock;
    private readonly CreateSessionCommandHandler _handler;

    public CreateSessionCommandHandlerTests()
    {
        _missionRepoMock = new Mock<IMissionRepository>();
        _sessionRepoMock = new Mock<ISessionRepository>();
        _handler = new CreateSessionCommandHandler(_missionRepoMock.Object, _sessionRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessWithPin()
    {
        // Arrange
        var missionId = Guid.NewGuid();
        var mission = new Mission(
            missionId,
            MissionTitle.Create("Test Mission"),
            null,
            MissionDifficulty.Media,
            30,
            MissionType.Tesoro);

        // Use reflection to set mission status to Activa since private setter
        typeof(Mission).GetProperty(nameof(Mission.Status))?.SetValue(mission, MissionStatus.Activa);

        _missionRepoMock
            .Setup(r => r.GetByIdAsync(missionId))
            .ReturnsAsync(mission);

        _sessionRepoMock
            .Setup(r => r.ExistsByNameWithActiveStatusAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _sessionRepoMock
            .Setup(r => r.IsPinUniqueAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var command = new CreateSessionCommand("Sesión Alpha", missionId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().NotBeEmpty();
        result.Pin.Should().NotBeNullOrEmpty();
        result.Pin.Length.Should().Be(6);
        result.Pin.Should().MatchRegex("^[0-9]{6}$");

        _sessionRepoMock.Verify(r => r.AddAsync(It.Is<Session>(s =>
            s.Name == "Sesión Alpha" &&
            s.MissionId == missionId &&
            s.Status == SessionStatus.Programada &&
            s.Pin.Length == 6)), Times.Once);
    }

    [Fact]
    public async Task Handle_MissionNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var missionId = Guid.NewGuid();

        _missionRepoMock
            .Setup(r => r.GetByIdAsync(missionId))
            .ReturnsAsync((Mission?)null);

        var command = new CreateSessionCommand("Sesión Alpha", missionId);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Mission with ID {missionId} not found.");

        _sessionRepoMock.Verify(r => r.AddAsync(It.IsAny<Session>()), Times.Never);
    }

    [Fact]
    public async Task Handle_MissionNotActiva_ThrowsArgumentException()
    {
        // Arrange
        var missionId = Guid.NewGuid();
        var mission = new Mission(
            missionId,
            MissionTitle.Create("Test Mission"),
            null,
            MissionDifficulty.Media,
            30,
            MissionType.Tesoro);

        // Status remains Borrador (default) — not Activa

        _missionRepoMock
            .Setup(r => r.GetByIdAsync(missionId))
            .ReturnsAsync(mission);

        var command = new CreateSessionCommand("Sesión Alpha", missionId);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Mission must be in Activa status.*");

        _sessionRepoMock.Verify(r => r.AddAsync(It.IsAny<Session>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NameConflictsActive_ThrowsConflictException()
    {
        // Arrange
        var missionId = Guid.NewGuid();
        var mission = new Mission(
            missionId,
            MissionTitle.Create("Test Mission"),
            null,
            MissionDifficulty.Media,
            30,
            MissionType.Tesoro);

        typeof(Mission).GetProperty(nameof(Mission.Status))?.SetValue(mission, MissionStatus.Activa);

        _missionRepoMock
            .Setup(r => r.GetByIdAsync(missionId))
            .ReturnsAsync(mission);

        _sessionRepoMock
            .Setup(r => r.ExistsByNameWithActiveStatusAsync("Sesión Alpha"))
            .ReturnsAsync(true);

        var command = new CreateSessionCommand("Sesión Alpha", missionId);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("A session with the name 'Sesión Alpha' is already active or pending.");

        _sessionRepoMock.Verify(r => r.AddAsync(It.IsAny<Session>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NameConflictsPaused_ThrowsConflictException()
    {
        // Arrange
        var missionId = Guid.NewGuid();
        var mission = new Mission(
            missionId,
            MissionTitle.Create("Test Mission"),
            null,
            MissionDifficulty.Media,
            30,
            MissionType.Tesoro);

        typeof(Mission).GetProperty(nameof(Mission.Status))?.SetValue(mission, MissionStatus.Activa);

        _missionRepoMock
            .Setup(r => r.GetByIdAsync(missionId))
            .ReturnsAsync(mission);

        // Repository considers Pausada as an active/pending status
        _sessionRepoMock
            .Setup(r => r.ExistsByNameWithActiveStatusAsync("Sesión Pausada"))
            .ReturnsAsync(true);

        var command = new CreateSessionCommand("Sesión Pausada", missionId);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("A session with the name 'Sesión Pausada' is already active or pending.");

        _sessionRepoMock.Verify(r => r.AddAsync(It.IsAny<Session>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PinCollision_RetrySucceeds()
    {
        // Arrange
        var missionId = Guid.NewGuid();
        var mission = new Mission(
            missionId,
            MissionTitle.Create("Test Mission"),
            null,
            MissionDifficulty.Media,
            30,
            MissionType.Tesoro);

        typeof(Mission).GetProperty(nameof(Mission.Status))?.SetValue(mission, MissionStatus.Activa);

        _missionRepoMock
            .Setup(r => r.GetByIdAsync(missionId))
            .ReturnsAsync(mission);

        _sessionRepoMock
            .Setup(r => r.ExistsByNameWithActiveStatusAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // First call returns false (collision), second returns true (success)
        var callCount = 0;
        _sessionRepoMock
            .Setup(r => r.IsPinUniqueAsync(It.IsAny<string>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                return callCount >= 2;
            });

        var command = new CreateSessionCommand("Sesión Alpha", missionId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.SessionId.Should().NotBeEmpty();
        result.Pin.Should().NotBeNullOrEmpty();
        result.Pin.Length.Should().Be(6);

        // IsPinUniqueAsync was called at least twice
        _sessionRepoMock.Verify(r => r.IsPinUniqueAsync(It.IsAny<string>()), Times.AtLeast(2));
        _sessionRepoMock.Verify(r => r.AddAsync(It.IsAny<Session>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PinExhaustion_ThrowsException()
    {
        // Arrange
        var missionId = Guid.NewGuid();
        var mission = new Mission(
            missionId,
            MissionTitle.Create("Test Mission"),
            null,
            MissionDifficulty.Media,
            30,
            MissionType.Tesoro);

        typeof(Mission).GetProperty(nameof(Mission.Status))?.SetValue(mission, MissionStatus.Activa);

        _missionRepoMock
            .Setup(r => r.GetByIdAsync(missionId))
            .ReturnsAsync(mission);

        _sessionRepoMock
            .Setup(r => r.ExistsByNameWithActiveStatusAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Always returns false — all PINs collide
        _sessionRepoMock
            .Setup(r => r.IsPinUniqueAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var command = new CreateSessionCommand("Sesión Alpha", missionId);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Unable to generate a unique PIN after 10 attempts.");

        _sessionRepoMock.Verify(r => r.IsPinUniqueAsync(It.IsAny<string>()), Times.Exactly(10));
        _sessionRepoMock.Verify(r => r.AddAsync(It.IsAny<Session>()), Times.Never);
    }
}
