namespace Umbral.Application.DTOs.Users;

/// <summary>
/// Full user detail DTO for single-user view.
/// </summary>
public record UserDetail(
    Guid Id,
    string Name,
    string Email,
    string? Alias,
    string Role,
    string Status,
    Guid? TeamId,
    DateTime CreatedAt);
