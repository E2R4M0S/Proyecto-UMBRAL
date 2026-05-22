using Umbral.Domain.Enums;
using Umbral.Domain.ValueObjects;

namespace Umbral.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid? TeamId { get; private set; }
    public Team? Team { get; private set; }

    public string? Alias => _alias;

    private string? _alias;

    private User() { }

    public User(
        Guid id,
        string name,
        Email email,
        string passwordHash,
        UserRole role)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        Status = UserStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public User(
        Guid id,
        string name,
        Email email,
        string passwordHash,
        UserRole role,
        Alias alias)
        : this(id, name, email, passwordHash, role)
    {
        _alias = alias.Value;
    }

    public bool IsActive() => Status == UserStatus.Active;

    public void AssignToTeam(Team team)
    {
        TeamId = team.Id;
        Team = team;
    }

    public void RemoveFromTeam()
    {
        TeamId = null;
        Team = null;
    }
}
