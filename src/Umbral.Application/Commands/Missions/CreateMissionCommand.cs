using MediatR;
using Umbral.Application.DTOs.Missions;

namespace Umbral.Application.Commands.Missions;

public record CreateMissionCommand(string Title, string? Description, string Difficulty, int TimeMinutes)
    : IRequest<CreateMissionResponse>;
