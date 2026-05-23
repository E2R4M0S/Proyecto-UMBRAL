using Moq;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Users;
using Umbral.Application.Queries.Users;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Tests.UnitTests;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new GetUserByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingUser_ReturnsUserDetail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "John Doe",
            Email.Create("john@umbral.com"),
            "hash",
            UserRole.Participant,
            Alias.Create("john_doe"));

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("john@umbral.com", result.Email);
        Assert.Equal("john_doe", result.Alias);
        Assert.Equal("Participant", result.Role);
        Assert.Equal("Active", result.Status);
        Assert.Null(result.TeamId);
    }

    [Fact]
    public async Task Handle_ExistingUserWithoutAlias_ReturnsUserDetailWithNullAlias()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Jane Doe",
            Email.Create("jane@umbral.com"),
            "hash",
            UserRole.Operator);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var query = new GetUserByIdQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("Jane Doe", result.Name);
        Assert.Equal("jane@umbral.com", result.Email);
        Assert.Null(result.Alias);
        Assert.Equal("Operator", result.Role);
        Assert.Equal("Active", result.Status);
    }

    [Fact]
    public async Task Handle_NonExistentUser_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var query = new GetUserByIdQuery(userId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));

        Assert.Contains(userId.ToString(), exception.Message);
    }

    [Fact]
    public async Task Handle_UserNotFound_RepositoryCalledWithCorrectId()
    {
        // Arrange
        var userId = Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var query = new GetUserByIdQuery(userId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));

        _repositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }
}
