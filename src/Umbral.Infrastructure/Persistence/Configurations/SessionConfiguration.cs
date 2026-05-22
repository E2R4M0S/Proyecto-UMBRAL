using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umbral.Domain.Entities;

namespace Umbral.Infrastructure.Persistence.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.MissionId)
            .IsRequired();

        builder.Property(s => s.Pin)
            .IsRequired()
            .HasMaxLength(6);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.StartedAt);

        builder.Property(s => s.EndedAt);

        // Relationships
        builder.HasOne(s => s.Mission)
            .WithMany()
            .HasForeignKey(s => s.MissionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.SessionTeams)
            .WithOne(st => st.Session)
            .HasForeignKey(st => st.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(s => s.Pin)
            .IsUnique();
    }
}
