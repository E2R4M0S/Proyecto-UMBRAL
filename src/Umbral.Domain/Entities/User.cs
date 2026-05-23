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
    public string SecurityStamp { get; private set; } = null!;
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
        SecurityStamp = Guid.NewGuid().ToString();
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

    public void Deactivate()
    {
        if (Status == UserStatus.Inactive)
            throw new InvalidOperationException("User is already inactive.");

        Status = UserStatus.Inactive;
        SecurityStamp = Guid.NewGuid().ToString();
    }

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

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty.", nameof(newName));
        Name = newName;
    }

    public void UpdateAlias(Alias newAlias)
    {
        _alias = newAlias.Value;
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        SecurityStamp = Guid.NewGuid().ToString();
    }
}
