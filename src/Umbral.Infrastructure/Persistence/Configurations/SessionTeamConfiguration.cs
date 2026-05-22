using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Umbral.Domain.Entities;

namespace Umbral.Infrastructure.Persistence.Configurations;

public class SessionTeamConfiguration : IEntityTypeConfiguration<SessionTeam>
{
    public void Configure(EntityTypeBuilder<SessionTeam> builder)
    {
        builder.ToTable("SessionTeams");

        builder.HasKey(st => new { st.SessionId, st.TeamId });

        builder.Property(st => st.Score)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(st => st.CurrentStage);

        builder.Property(st => st.JoinedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(st => st.Session)
            .WithMany(s => s.SessionTeams)
            .HasForeignKey(st => st.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(st => st.Team)
            .WithMany()
            .HasForeignKey(st => st.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
