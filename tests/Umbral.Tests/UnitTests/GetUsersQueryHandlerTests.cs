using Moq;
using Umbral.Application.DTOs.Users;
using Umbral.Application.Queries.Users;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Primitives;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Tests.UnitTests;

public class GetUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new GetUsersQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsCorrectPageSize()
    {
        // Arrange
        var users = CreateUserList(25);
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, null, null, null))
            .ReturnsAsync(new PaginatedResult<User>(users.Take(10).ToList(), 1, 10, 25));

        var query = new GetUsersQuery();

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
    public async Task Handle_WithRoleFilter_DelegatesFilterToRepository()
    {
        // Arrange
        var users = CreateUserList(5);
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, "Operator", null, null))
            .ReturnsAsync(new PaginatedResult<User>(users.Take(2).ToList(), 1, 10, 2));

        var query = new GetUsersQuery(Role: "Operator");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Items.Count);
        _repositoryMock.Verify(r => r.GetAllAsync(1, 10, "Operator", null, null), Times.Once);
    }

    [Fact]
    public async Task Handle_WithStatusFilter_DelegatesFilterToRepository()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 5, null, "Inactive", null))
            .ReturnsAsync(new PaginatedResult<User>(new List<User>(), 1, 5, 0));

        var query = new GetUsersQuery(1, 5, Status: "Inactive");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        _repositoryMock.Verify(r => r.GetAllAsync(1, 5, null, "Inactive", null), Times.Once);
    }

    [Fact]
    public async Task Handle_WithSearchFilter_DelegatesFilterToRepository()
    {
        // Arrange
        var users = CreateUserList(3);
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, null, null, "john"))
            .ReturnsAsync(new PaginatedResult<User>(users.Take(1).ToList(), 1, 10, 1));

        var query = new GetUsersQuery(Search: "john");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result.Items);
        _repositoryMock.Verify(r => r.GetAllAsync(1, 10, null, null, "john"), Times.Once);
    }

    [Fact]
    public async Task Handle_WithMultipleFilters_PassesAllToRepository()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 5, "Participant", "Active", "test"))
            .ReturnsAsync(new PaginatedResult<User>(new List<User>(), 1, 5, 0));

        var query = new GetUsersQuery(1, 5, "Participant", "Active", "test");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        _repositoryMock.Verify(r => r.GetAllAsync(1, 5, "Participant", "Active", "test"), Times.Once);
    }

    [Fact]
    public async Task Handle_MapsUserToUserListItem_Correctly()
    {
        // Arrange
        var user = CreateUser(
            Guid.NewGuid(),
            "John Doe",
            "john@umbral.com",
            UserRole.Participant,
            UserStatus.Active);

        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, null, null, null))
            .ReturnsAsync(new PaginatedResult<User>(new List<User> { user }, 1, 10, 1));

        var query = new GetUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var item = Assert.Single(result.Items);
        Assert.Equal(user.Id, item.Id);
        Assert.Equal("John Doe", item.Name);
        Assert.Equal("john@umbral.com", item.Email);
        Assert.Equal("Participant", item.Role);
        Assert.Equal("Active", item.Status);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyResult()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, null, null, null))
            .ReturnsAsync(PaginatedResult<User>.Empty);

        var query = new GetUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    private static List<User> CreateUserList(int count)
    {
        var users = new List<User>();
        for (int i = 0; i < count; i++)
        {
            var role = (UserRole)(i % 3);
            var status = i % 2 == 0 ? UserStatus.Active : UserStatus.Inactive;
            users.Add(CreateUser(
                Guid.NewGuid(),
                $"User {i + 1}",
                $"user{i + 1}@umbral.com",
                role,
                status));
        }
        return users;
    }

    private static User CreateUser(Guid id, string name, string email, UserRole role, UserStatus status)
    {
        var user = new User(id, name, Email.Create(email), "hash", role);

        // Use reflection to set Status since it has a private setter
        var statusProperty = typeof(User).GetProperty(nameof(User.Status))!;
        statusProperty.SetValue(user, status);

        return user;
    }
}
