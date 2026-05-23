using FluentAssertions;
using Moq;
using Umbral.Application.Commands.Participants;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.Common.Interfaces;
using Umbral.Application.DTOs.Participants;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Tests.UnitTests;

public class UpdateParticipantProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly UpdateParticipantProfileCommandHandler _handler;

    public UpdateParticipantProfileCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _handler = new UpdateParticipantProfileCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object);
    }

    private static User CreateParticipant()
    {
        return new User(
            Guid.NewGuid(),
            "Original Name",
            Email.Create("participant@umbral.com"),
            "hash",
            UserRole.Participant,
            Alias.Create("original_alias"));
    }

    [Fact]
    public async Task Handle_UpdateName_ReturnsNewToken()
    {
        // Arrange
        var user = CreateParticipant();
        var newName = "Updated Name";
        var expiresAt = DateTime.UtcNow.AddMinutes(60);

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(t => t.GenerateToken(user, out expiresAt))
            .Returns("new-token");

        var command = new UpdateParticipantProfileCommand(
            user.Id, newName, null, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be(newName);
        result.Token.Should().Be("new-token");
        result.ExpiresAt.Should().NotBeNull();

        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<User>(), out It.Ref<DateTime>.IsAny), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateAlias_UniqueAlias_Success()
    {
        // Arrange
        var user = CreateParticipant();
        var newAlias = "new_alias";

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(r => r.GetByAliasAsync(It.Is<Alias>(a => a.Value == newAlias)))
            .ReturnsAsync((User?)null);

        var command = new UpdateParticipantProfileCommand(
            user.Id, null, newAlias, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Alias.Should().Be(newAlias);
        result.Token.Should().BeNull(); // Alias change only — no token refresh
        result.ExpiresAt.Should().BeNull();

        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<User>(), out It.Ref<DateTime>.IsAny), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateAlias_DuplicateAlias_ThrowsConflictException()
    {
        // Arrange
        var user = CreateParticipant();
        var otherUser = CreateParticipant();
        var duplicateAlias = "duplicate";

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(r => r.GetByAliasAsync(It.Is<Alias>(a => a.Value == duplicateAlias)))
            .ReturnsAsync(otherUser); // Different user has this alias

        var command = new UpdateParticipantProfileCommand(
            user.Id, null, duplicateAlias, null, null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage($"*'{duplicateAlias}'*");

        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateAlias_SameAlias_Success()
    {
        // Arrange
        var user = CreateParticipant();
        var sameAlias = "original_alias"; // Already the user's alias

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        // The "conflicting" user IS the same user — should be allowed
        _userRepositoryMock
            .Setup(r => r.GetByAliasAsync(It.Is<Alias>(a => a.Value == sameAlias)))
            .ReturnsAsync(user);

        var command = new UpdateParticipantProfileCommand(
            user.Id, null, sameAlias, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Alias.Should().Be(sameAlias);
        result.Token.Should().BeNull();

        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ChangePassword_ValidCurrent_Success()
    {
        // Arrange
        var user = CreateParticipant();
        var currentPassword = "currentPass123";
        var newPassword = "newPass456";
        var newHash = "new-hash";
        var expiresAt = DateTime.UtcNow.AddMinutes(60);

        // Store original stamp for comparison
        var originalStamp = user.SecurityStamp;

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(p => p.Verify(currentPassword, user.PasswordHash))
            .Returns(true);

        _passwordHasherMock
            .Setup(p => p.Hash(newPassword))
            .Returns(newHash);

        _tokenServiceMock
            .Setup(t => t.GenerateToken(It.IsAny<User>(), out expiresAt))
            .Returns("new-token-after-pw-change");

        var command = new UpdateParticipantProfileCommand(
            user.Id, null, null, currentPassword, newPassword);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.PasswordHash.Should().Be(newHash);
        user.SecurityStamp.Should().NotBe(originalStamp);
        result.Token.Should().Be("new-token-after-pw-change");
        result.ExpiresAt.Should().NotBeNull();

        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<User>(), out It.Ref<DateTime>.IsAny), Times.Once);
    }

    [Fact]
    public async Task Handle_ChangePassword_WrongCurrent_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = CreateParticipant();
        var wrongCurrentPassword = "wrongPass";
        var newPassword = "newPass456";

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(p => p.Verify(wrongCurrentPassword, user.PasswordHash))
            .Returns(false);

        var command = new UpdateParticipantProfileCommand(
            user.Id, null, null, wrongCurrentPassword, newPassword);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Current password is incorrect.");

        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<User>(), out It.Ref<DateTime>.IsAny), Times.Never);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var command = new UpdateParticipantProfileCommand(
            userId, "New Name", null, null, null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found.");

        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UserIsNotParticipant_ThrowsInvalidOperationException()
    {
        // Arrange
        var operatorUser = new User(
            Guid.NewGuid(),
            "Operator User",
            Email.Create("operator@umbral.com"),
            "hash",
            UserRole.Operator);

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(operatorUser.Id))
            .ReturnsAsync(operatorUser);

        var command = new UpdateParticipantProfileCommand(
            operatorUser.Id, "New Name", null, null, null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Only participants can update their profile.");

        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}
