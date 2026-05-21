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

    public bool IsActive() => Status == UserStatus.Active;
}
