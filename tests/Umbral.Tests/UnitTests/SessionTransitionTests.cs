using FluentAssertions;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;

namespace Umbral.Tests.UnitTests;

public class SessionTransitionTests
{
    private static Session CreateSessionWithStatus(SessionStatus status)
    {
        var session = new Session(Guid.NewGuid(), "Test Session", Guid.NewGuid(), "123456");
        if (status != SessionStatus.Programada)
        {
            typeof(Session)
                .GetProperty(nameof(Session.Status))!
                .SetValue(session, status);
        }
        return session;
    }

    [Fact]
    public void MoveToPreparing_FromProgramada_SetsEnPreparacion()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Programada);

        // Act
        session.MoveToPreparing();

        // Assert
        session.Status.Should().Be(SessionStatus.EnPreparacion);
    }

    [Fact]
    public void Activate_FromEnPreparacion_SetsStartedAt()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.EnPreparacion);

        // Act
        session.Activate();

        // Assert
        session.Status.Should().Be(SessionStatus.Activa);
        session.StartedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_FromPausada_DoesNotOverwriteStartedAt()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Pausada);
        var originalStartedAt = DateTime.UtcNow.AddMinutes(-30);
        typeof(Session).GetProperty(nameof(Session.StartedAt))!.SetValue(session, originalStartedAt);

        // Act
        session.Activate();

        // Assert
        session.Status.Should().Be(SessionStatus.Activa);
        session.StartedAt.Should().Be(originalStartedAt);
    }

    [Fact]
    public void Finalize_FromActiva_SetsEndedAt()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Activa);

        // Act
        session.Finalize();

        // Assert
        session.Status.Should().Be(SessionStatus.Finalizada);
        session.EndedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_FromProgramada_SetsCancelada()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Programada);

        // Act
        session.Cancel();

        // Assert
        session.Status.Should().Be(SessionStatus.Cancelada);
    }

    [Fact]
    public void MoveToPreparing_FromActiva_Throws()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Activa);

        // Act
        var act = () => session.MoveToPreparing();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot transition*EnPreparacion*");
    }

    [Fact]
    public void Activate_FromFinalizada_Throws()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Finalizada);

        // Act
        var act = () => session.Activate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot transition*Activa*");
    }

    [Fact]
    public void Cancel_FromCancelada_Throws()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Cancelada);

        // Act
        var act = () => session.Cancel();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot cancel*Cancelada*");
    }

    [Fact]
    public void Pause_FromActiva_SetsPausada()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Activa);

        // Act
        session.Pause();

        // Assert
        session.Status.Should().Be(SessionStatus.Pausada);
    }

    [Fact]
    public void Pause_FromProgramada_Throws()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Programada);

        // Act
        var act = () => session.Pause();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot transition*Pausada*");
    }

    [Fact]
    public void Resume_FromPausada_RestoresActiva()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Pausada);
        var originalStartedAt = DateTime.UtcNow.AddMinutes(-30);
        typeof(Session).GetProperty(nameof(Session.StartedAt))!.SetValue(session, originalStartedAt);

        // Act
        session.Resume();

        // Assert
        session.Status.Should().Be(SessionStatus.Activa);
        session.StartedAt.Should().Be(originalStartedAt); // not overwritten
    }

    [Fact]
    public void Finalize_FromPausada_SetsEndedAt()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Pausada);

        // Act
        session.Finalize();

        // Assert
        session.Status.Should().Be(SessionStatus.Finalizada);
        session.EndedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_FromActiva_SetsCancelada()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Activa);

        // Act
        session.Cancel();

        // Assert
        session.Status.Should().Be(SessionStatus.Cancelada);
    }

    [Fact]
    public void Cancel_FromFinalizada_Throws()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Finalizada);

        // Act
        var act = () => session.Cancel();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot cancel*Finalizada*");
    }

    [Fact]
    public void Finalize_FromFinalizada_Throws()
    {
        // Arrange
        var session = CreateSessionWithStatus(SessionStatus.Finalizada);

        // Act
        var act = () => session.Finalize();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot transition*Finalizada*");
    }
}
