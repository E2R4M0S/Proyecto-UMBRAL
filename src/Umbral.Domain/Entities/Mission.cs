using Umbral.Domain.Enums;
using Umbral.Domain.ValueObjects;

namespace Umbral.Domain.Entities;

public class Mission
{
    public Guid Id { get; private set; }
    public MissionTitle Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public MissionDifficulty Difficulty { get; private set; }
    public int TimeMinutes { get; private set; }
    public MissionType Type { get; private set; }
    public MissionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Mission() { }

    public Mission(
        Guid id,
        MissionTitle title,
        string? description,
        MissionDifficulty difficulty,
        int timeMinutes,
        MissionType type)
    {
        Id = id;
        Title = title;
        Description = description;
        Difficulty = difficulty;
        TimeMinutes = timeMinutes;
        Type = type;
        Status = MissionStatus.Borrador;
        CreatedAt = DateTime.UtcNow;
    }
}
