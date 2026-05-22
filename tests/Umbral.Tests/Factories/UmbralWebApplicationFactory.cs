using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Umbral.Infrastructure.Persistence;

namespace Umbral.Tests.Factories;

/// <summary>
/// Custom WebApplicationFactory that replaces PostgreSQL with InMemory database
/// and configures test authentication.
/// </summary>
public class UmbralWebApplicationFactory : WebApplicationFactory<Program>
{
    public string DatabaseName { get; }

    public UmbralWebApplicationFactory()
    {
        DatabaseName = Guid.NewGuid().ToString("N");

        // Set environment variable BEFORE host starts building
        Environment.SetEnvironmentVariable("UseInMemoryDatabase", "true");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Also set via WebHost setting as a fallback
        builder.UseSetting("UseInMemoryDatabase", "true");

        builder.ConfigureTestServices(services =>
        {
            // Override default auth scheme to use test handler.
            // The test handler reads X-Test-Role header; absence = 401, presence = authenticated with role.
            services.RemoveAll<IAuthenticationSchemeProvider>();
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { });
        });
    }
}
