namespace Umbral.Application.DTOs.Users;

/// <summary>
/// Lightweight DTO for user catalog list view.
/// </summary>
public record UserListItem(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Status);
