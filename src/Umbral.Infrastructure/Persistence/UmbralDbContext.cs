using Microsoft.EntityFrameworkCore;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;

namespace Umbral.Infrastructure.Persistence;

public class UmbralDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public UmbralDbContext(DbContextOptions<UmbralDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UmbralDbContext).Assembly);

        SeedDefaultAdmin(modelBuilder);
    }

    private static void SeedDefaultAdmin(ModelBuilder modelBuilder)
    {
        var adminId = Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");

        modelBuilder.Entity<User>().HasData(new
        {
            Id = adminId,
            Name = "Admin",
            Email = "admin@umbral.com",
            PasswordHash = "$2a$11$OPhzEcSRAoqFm/ekU/Dq0OQOXRjZVIG35YsYo.VCFfUPuaaQYtIce",
            Role = UserRole.Admin.ToString(),
            Status = UserStatus.Active.ToString(),
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
