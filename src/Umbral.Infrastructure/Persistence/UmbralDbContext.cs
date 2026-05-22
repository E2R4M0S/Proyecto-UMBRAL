using Microsoft.EntityFrameworkCore;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.ValueObjects;

namespace Umbral.Infrastructure.Persistence;

public class UmbralDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Mission> Missions => Set<Mission>();
    public DbSet<Team> Teams => Set<Team>();

    public UmbralDbContext(DbContextOptions<UmbralDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UmbralDbContext).Assembly);
    }

    public async Task SeedAsync()
    {
        if (await Users.AnyAsync())
            return;

        Users.Add(new User(
            Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890"),
            "Admin",
            Email.Create("admin@umbral.com"),
            "$2a$11$OPhzEcSRAoqFm/ekU/Dq0OQOXRjZVIG35YsYo.VCFfUPuaaQYtIce",
            UserRole.Admin));

        await SaveChangesAsync();
    }
}
