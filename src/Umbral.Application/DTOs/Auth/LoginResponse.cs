namespace Umbral.Application.DTOs.Auth;

public record LoginResponse(
    string Token,
    DateTime ExpiresAt,
    Guid UserId,
    string UserName,
    string Email,
    string Role
);
