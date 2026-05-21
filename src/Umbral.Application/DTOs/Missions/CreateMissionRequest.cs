namespace Umbral.Application.DTOs.Missions;

public record CreateMissionRequest(string Title, string? Description, string Difficulty, int TimeMinutes);
