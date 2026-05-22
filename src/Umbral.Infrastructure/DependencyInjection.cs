using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbral.Application.Common.Interfaces;
using Umbral.Domain.Repositories;
using Umbral.Infrastructure.Persistence;
using Umbral.Infrastructure.Persistence.Repositories;
using Umbral.Infrastructure.Services;

namespace Umbral.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Allow test project to override the database provider via config
        if (string.Equals(configuration["UseInMemoryDatabase"], "true", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<UmbralDbContext>(options =>
                options.UseInMemoryDatabase("UmbralTestDb"));
        }
        else
        {
            services.AddDbContext<UmbralDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMissionRepository, MissionRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
