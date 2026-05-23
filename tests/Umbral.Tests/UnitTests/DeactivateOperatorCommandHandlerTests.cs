using FluentAssertions;
using Moq;
using Umbral.Application.Commands.Operators;
using Umbral.Application.Common.Exceptions;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Tests.UnitTests;

public class DeactivateOperatorCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly DeactivateOperatorCommandHandler _handler;

    public DeactivateOperatorCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new DeactivateOperatorCommandHandler(_repositoryMock.Object);
    }

    private static User CreateActiveOperator()
    {
        return new User(
            Guid.NewGuid(),
            "Test Operator",
            Email.Create("operator@umbral.com"),
            "hash",
            UserRole.Operator);
    }

    [Fact]
    public async Task Handle_OperatorExists_DeactivatesAndUpdates()
    {
        // Arrange
        var user = CreateActiveOperator();
        var originalStamp = user.SecurityStamp;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        var command = new DeactivateOperatorCommand(user.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.IsActive().Should().BeFalse();
        user.SecurityStamp.Should().NotBe(originalStamp);
        _repositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_OperatorNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var operatorId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(operatorId))
            .ReturnsAsync((User?)null);

        var command = new DeactivateOperatorCommand(operatorId);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*'{operatorId}'*");

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UserIsNotOperator_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test Participant",
            Email.Create("participant@umbral.com"),
            "hash",
            UserRole.Participant);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        var command = new DeactivateOperatorCommand(user.Id);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User is not an Operator.");

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AlreadyInactive_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = CreateActiveOperator();
        user.Deactivate(); // First deactivation succeeds

        // Reset mock to return already-inactive user
        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        var command = new DeactivateOperatorCommand(user.Id);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User is already inactive.");

        // UpdateAsync should not be called because Deactivate() throws before reaching UpdateAsync
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}
