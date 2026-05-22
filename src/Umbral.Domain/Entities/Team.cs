using Umbral.Domain.Enums;
using Umbral.Domain.ValueObjects;

namespace Umbral.Domain.Entities;

public class Team
{
    public Guid Id { get; private set; }
    public TeamName Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid LeaderId { get; private set; }
    public int Score { get; private set; }
    public TeamStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public User Leader { get; private set; } = null!;
    public ICollection<User> Members { get; private set; } = new List<User>();

    private Team() { }

    public Team(
        Guid id,
        TeamName name,
        string? description,
        Guid leaderId)
    {
        Id = id;
        Name = name;
        Description = description;
        LeaderId = leaderId;
        Score = 0;
        Status = TeamStatus.Activo;
        CreatedAt = DateTime.UtcNow;
    }
}
