using FluentAssertions;
using Moq;
using Umbral.Application.Commands.Sessions;
using Umbral.Application.Common.Exceptions;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;

namespace Umbral.Tests.UnitTests;

public class UpdateSessionStatusCommandHandlerTests
{
    private readonly Mock<ISessionRepository> _repositoryMock;
    private readonly UpdateSessionStatusCommandHandler _handler;

    public UpdateSessionStatusCommandHandlerTests()
    {
        _repositoryMock = new Mock<ISessionRepository>();
        _handler = new UpdateSessionStatusCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_SessionNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync((Session?)null);

        var command = new UpdateSessionStatusCommand(sessionId, "EnPreparacion");

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*'{sessionId}'*");

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Session>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidTransition_UpdatesAndSaves()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session(sessionId, "Test Session", Guid.NewGuid(), "123456");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);

        var command = new UpdateSessionStatusCommand(sessionId, "EnPreparacion");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        session.Status.Should().Be(SessionStatus.EnPreparacion);
        _repositoryMock.Verify(r => r.UpdateAsync(session), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidTransition_Activa_SetsStartedAt()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session(sessionId, "Test Session", Guid.NewGuid(), "123456");
        typeof(Session).GetProperty(nameof(Session.Status))!.SetValue(session, SessionStatus.EnPreparacion);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);

        var command = new UpdateSessionStatusCommand(sessionId, "Activa");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        session.Status.Should().Be(SessionStatus.Activa);
        session.StartedAt.Should().NotBeNull();
        _repositoryMock.Verify(r => r.UpdateAsync(session), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidTransition_ThrowsInvalidOperationException()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session(sessionId, "Test Session", Guid.NewGuid(), "123456");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);

        // Session starts as Programada — cannot transition directly to Activa
        var command = new UpdateSessionStatusCommand(sessionId, "Activa");

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Cannot transition*Activa*");

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Session>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CancelFromProgramada_Succeeds()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session(sessionId, "Test Session", Guid.NewGuid(), "123456");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);

        var command = new UpdateSessionStatusCommand(sessionId, "Cancelada");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        session.Status.Should().Be(SessionStatus.Cancelada);
        _repositoryMock.Verify(r => r.UpdateAsync(session), Times.Once);
    }

    [Fact]
    public async Task Handle_FinalizeFromActiva_SetsEndedAt()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session(sessionId, "Test Session", Guid.NewGuid(), "123456");
        typeof(Session).GetProperty(nameof(Session.Status))!.SetValue(session, SessionStatus.Activa);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);

        var command = new UpdateSessionStatusCommand(sessionId, "Finalizada");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        session.Status.Should().Be(SessionStatus.Finalizada);
        session.EndedAt.Should().NotBeNull();
        _repositoryMock.Verify(r => r.UpdateAsync(session), Times.Once);
    }
}
