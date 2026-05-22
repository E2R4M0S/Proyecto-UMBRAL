using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Primitives;
using Umbral.Domain.ValueObjects;
using Umbral.Infrastructure.Persistence;
using Umbral.Infrastructure.Persistence.Repositories;
using Umbral.Tests.Factories;

namespace Umbral.Tests.IntegrationTests;

public class MissionRepositoryIntegrationTests : IClassFixture<UmbralWebApplicationFactory>
{
    private readonly UmbralWebApplicationFactory _factory;

    public MissionRepositoryIntegrationTests(UmbralWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<(UmbralDbContext Context, MissionRepository Repository)> CreateSutAsync()
    {
        // Create a fresh InMemory database per test to avoid data leakage between tests
        var options = new DbContextOptionsBuilder<UmbralDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid():N}")
            .Options;

        var context = new UmbralDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new MissionRepository(context);
        return (context, repository);
    }

    [Fact]
    public async Task GetAllAsync_WithPagination_ReturnsCorrectSlice()
    {
        // Arrange
        var (context, repository) = await CreateSutAsync();
        context.Missions.AddRange(SeedMissions(25));
        await context.SaveChangesAsync();

        // Act - first page
        var result = await repository.GetAllAsync(1, 10);

        // Assert
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(3, result.TotalPages);

        // Act - second page
        var page2 = await repository.GetAllAsync(2, 10);
        Assert.Equal(10, page2.Items.Count);
        Assert.Equal(2, page2.Page);

        // Act - third page (last)
        var page3 = await repository.GetAllAsync(3, 10);
        Assert.Equal(5, page3.Items.Count);
        Assert.Equal(3, page3.Page);
    }

    [Fact]
    public async Task GetAllAsync_FilterByDifficulty_ReturnsOnlyMatching()
    {
        // Arrange
        var (context, repository) = await CreateSutAsync();
        context.Missions.AddRange(
            CreateMission("Facil 1", MissionDifficulty.Facil, MissionStatus.Activa),
            CreateMission("Facil 2", MissionDifficulty.Facil, MissionStatus.Borrador),
            CreateMission("Media 1", MissionDifficulty.Media, MissionStatus.Activa),
            CreateMission("Dificil 1", MissionDifficulty.Dificil, MissionStatus.Activa));
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync(1, 10, difficulty: "Facil");

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, m => Assert.Equal(MissionDifficulty.Facil, m.Difficulty));
    }

    [Fact]
    public async Task GetAllAsync_FilterByStatus_ReturnsOnlyMatching()
    {
        // Arrange
        var (context, repository) = await CreateSutAsync();
        context.Missions.AddRange(
            CreateMission("M1", MissionDifficulty.Facil, MissionStatus.Activa),
            CreateMission("M2", MissionDifficulty.Media, MissionStatus.Activa),
            CreateMission("M3", MissionDifficulty.Dificil, MissionStatus.Borrador));
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync(1, 10, status: "Activa");

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, m => Assert.Equal(MissionStatus.Activa, m.Status));
    }

    [Fact]
    public async Task GetAllAsync_SearchByTitle_ReturnsPartialMatch()
    {
        // Arrange
        var (context, repository) = await CreateSutAsync();
        context.Missions.AddRange(
            CreateMission("Rescate en montaña", MissionDifficulty.Dificil, MissionStatus.Activa),
            CreateMission("Búsqueda urbana", MissionDifficulty.Facil, MissionStatus.Activa),
            CreateMission("Rescate acuático", MissionDifficulty.Media, MissionStatus.Borrador));
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync(1, 10, search: "rescate");

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, m => m.Title.Value.Contains("montaña"));
        Assert.Contains(result.Items, m => m.Title.Value.Contains("acuático"));
    }

    [Fact]
    public async Task GetAllAsync_CombinedFilters_ApplySimultaneously()
    {
        // Arrange
        var (context, repository) = await CreateSutAsync();
        context.Missions.AddRange(
            CreateMission("Rescate fácil activa", MissionDifficulty.Facil, MissionStatus.Activa),
            CreateMission("Rescate fácil borrador", MissionDifficulty.Facil, MissionStatus.Borrador),
            CreateMission("Rescate difícil activa", MissionDifficulty.Dificil, MissionStatus.Activa),
            CreateMission("Otra activa", MissionDifficulty.Facil, MissionStatus.Activa));
        await context.SaveChangesAsync();

        // Act — search=rescate, difficulty=Facil, status=Activa
        var result = await repository.GetAllAsync(1, 10, difficulty: "Facil", status: "Activa", search: "rescate");

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("Rescate fácil activa", result.Items[0].Title.Value);
    }

    [Fact]
    public async Task GetAllAsync_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var (context, repository) = await CreateSutAsync();
        context.Missions.AddRange(SeedMissions(5));
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync(1, 10, difficulty: "Nonexistent");

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public async Task GetAllAsync_ClampsPageSizeToMax50()
    {
        // Arrange
        var (context, repository) = await CreateSutAsync();
        context.Missions.AddRange(SeedMissions(100));
        await context.SaveChangesAsync();

        // Act — pageSize > 50 should be clamped to 50
        var result = await repository.GetAllAsync(1, 100);

        // Assert
        Assert.Equal(50, result.Items.Count);
        Assert.Equal(1, result.Page);
        Assert.Equal(50, result.PageSize);
    }

    [Fact]
    public async Task GetAllAsync_PageLessThan1_DefaultsTo1()
    {
        // Arrange
        var (context, repository) = await CreateSutAsync();
        context.Missions.AddRange(SeedMissions(10));
        await context.SaveChangesAsync();

        // Act — page 0 should be clamped to 1
        var result = await repository.GetAllAsync(0, 10);

        // Assert
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(1, result.Page);
    }

    private static List<Mission> SeedMissions(int count)
    {
        var missions = new List<Mission>();
        for (int i = 0; i < count; i++)
        {
            missions.Add(CreateMission(
                $"Mission {i + 1}",
                (MissionDifficulty)(i % 3),
                i % 2 == 0 ? MissionStatus.Borrador : MissionStatus.Activa));
        }
        return missions;
    }

    private static Mission CreateMission(string title, MissionDifficulty difficulty, MissionStatus status)
    {
        var mission = new Mission(Guid.NewGuid(), MissionTitle.Create(title), $"Description for {title}", difficulty, 30);
        var statusProp = typeof(Mission).GetProperty("Status")!;
        statusProp.SetValue(mission, status);
        return mission;
    }
}
