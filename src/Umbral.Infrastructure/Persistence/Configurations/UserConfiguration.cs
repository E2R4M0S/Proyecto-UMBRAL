using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.ValueObjects;

namespace Umbral.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(
                v => v.Value,
                v => Email.Create(v));

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.SecurityStamp)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.Alias)
            .HasMaxLength(20);

        builder.Property(u => u.TeamId);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasIndex(u => u.Alias)
            .IsUnique()
            .HasFilter("\"Alias\" IS NOT NULL");
    }
}
