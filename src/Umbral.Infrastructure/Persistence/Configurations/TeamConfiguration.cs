using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.ValueObjects;

namespace Umbral.Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion(
                v => v.Value,
                v => TeamName.Create(v));

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.LeaderId)
            .IsRequired();

        builder.Property(t => t.Score)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        // Unique index on Name (case-insensitive via ILike for PostgreSQL)
        builder.HasIndex(t => t.Name)
            .IsUnique()
            .HasDatabaseName("IX_Teams_Name");

        // Leader: 1 User, no navigation collection on User side
        builder.HasOne(t => t.Leader)
            .WithMany()
            .HasForeignKey(t => t.LeaderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Members: 1:N via User.TeamId
        builder.HasMany(t => t.Members)
            .WithOne(u => u.Team)
            .HasForeignKey(u => u.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
