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

    public void MoveToPreparing()
    {
        if (Status != SessionStatus.Programada)
            throw new InvalidOperationException($"Cannot transition from {Status} to EnPreparacion.");
        Status = SessionStatus.EnPreparacion;
    }

    public void Activate()
    {
        if (Status != SessionStatus.EnPreparacion && Status != SessionStatus.Pausada)
            throw new InvalidOperationException($"Cannot transition from {Status} to Activa.");
        Status = SessionStatus.Activa;
        StartedAt ??= DateTime.UtcNow;
    }

    public void Pause()
    {
        if (Status != SessionStatus.Activa)
            throw new InvalidOperationException($"Cannot transition from {Status} to Pausada.");
        Status = SessionStatus.Pausada;
    }

    public void Resume() => Activate();

#pragma warning disable CS0465 // 'Finalize' is a reserved method name in .NET
    public void Finalize()
#pragma warning restore CS0465
    {
        if (Status != SessionStatus.Activa && Status != SessionStatus.Pausada)
            throw new InvalidOperationException($"Cannot transition from {Status} to Finalizada.");
        Status = SessionStatus.Finalizada;
        EndedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status is SessionStatus.Finalizada or SessionStatus.Cancelada)
            throw new InvalidOperationException($"Cannot cancel a session in {Status} state.");
        Status = SessionStatus.Cancelada;
    }
}
