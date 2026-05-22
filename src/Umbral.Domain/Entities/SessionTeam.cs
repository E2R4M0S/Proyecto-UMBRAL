namespace Umbral.Domain.Entities;

public class SessionTeam
{
    public Guid SessionId { get; private set; }
    public Guid TeamId { get; private set; }
    public int Score { get; private set; }
    public int? CurrentStage { get; private set; }
    public DateTime JoinedAt { get; private set; }

    // Navigation properties
    public Session Session { get; private set; } = null!;
    public Team Team { get; private set; } = null!;

    private SessionTeam() { }

    public SessionTeam(Guid sessionId, Guid teamId)
    {
        SessionId = sessionId;
        TeamId = teamId;
        Score = 0;
        JoinedAt = DateTime.UtcNow;
    }
}
