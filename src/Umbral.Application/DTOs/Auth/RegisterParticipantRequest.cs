namespace Umbral.Application.DTOs.Auth;

public record RegisterParticipantRequest(string Name, string Alias, string Email, string Password);
