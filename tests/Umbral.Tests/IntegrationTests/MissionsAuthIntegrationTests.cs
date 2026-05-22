using System.Net;
using System.Net.Http.Json;
using Umbral.Tests.Factories;

namespace Umbral.Tests.IntegrationTests;

public class MissionsAuthIntegrationTests : IClassFixture<UmbralWebApplicationFactory>
{
    private readonly UmbralWebApplicationFactory _factory;

    public MissionsAuthIntegrationTests(UmbralWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMissions_WithoutToken_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/missions");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMissionById_WithoutToken_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/missions/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMissions_WithAdminRole_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", "Admin");

        // Act
        var response = await client.GetAsync("/api/missions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMissions_WithOperatorRole_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", "Operator");

        // Act
        var response = await client.GetAsync("/api/missions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMissions_WithParticipantRole_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", "Participant");

        // Act
        var response = await client.GetAsync("/api/missions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMissionById_WithAnyRole_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", "Operator");

        // Act
        var response = await client.GetAsync($"/api/missions/{Guid.NewGuid()}");

        // Note: 404 is acceptable here since the mission doesn't exist
        // The key assertion is that we're authenticated and NOT 401
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateMission_AsNonAdmin_Returns403()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", "Operator");

        var request = new
        {
            Title = "New Mission",
            Difficulty = "Facil",
            TimeMinutes = 30
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/missions", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateMission_AsAdmin_DoesNotReturn403()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", "Admin");

        var request = new
        {
            Title = "Admin created mission",
            Difficulty = "Facil",
            TimeMinutes = 30
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/missions", request);

        // Assert
        // Should not be forbidden — may return 400 due to validation or 201 on success
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
