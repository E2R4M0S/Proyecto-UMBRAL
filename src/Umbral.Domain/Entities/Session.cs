using Umbral.Domain.Enums;

namespace Umbral.Domain.Entities;

public class Session
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Guid MissionId { get; private set; }
    public string Pin { get; private set; } = null!;
    public SessionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    // Navigation properties
    public Mission Mission { get; private set; } = null!;
    public ICollection<SessionTeam> SessionTeams { get; private set; } = new List<SessionTeam>();

    private Session() { }

    public Session(Guid id, string name, Guid missionId, string pin)
    {
        Id = id;
        Name = name;
        MissionId = missionId;
        Pin = pin;
        Status = SessionStatus.Programada;
        CreatedAt = DateTime.UtcNow;
    }
}
