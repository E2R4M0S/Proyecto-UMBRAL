using MediatR;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Missions;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Application.Commands.Missions;

public class CreateMissionCommandHandler : IRequestHandler<CreateMissionCommand, CreateMissionResponse>
{
    private readonly IMissionRepository _missionRepository;

    public CreateMissionCommandHandler(IMissionRepository missionRepository)
    {
        _missionRepository = missionRepository;
    }

    public async Task<CreateMissionResponse> Handle(CreateMissionCommand request, CancellationToken cancellationToken)
    {
        var title = MissionTitle.Create(request.Title);

        var exists = await _missionRepository.ExistsByTitleAsync(title);
        if (exists)
            throw new ConflictException("A mission with this title already exists.");

        // Defense-in-depth: validate TimeMinutes even though FluentValidation already checks
        if (request.TimeMinutes is not (15 or 30 or 60 or 90))

            throw new ArgumentException("TimeMinutes must be 15, 30, 60, or 90.", nameof(request.TimeMinutes));
        var difficulty = request.Difficulty switch
        {
            "Facil" => MissionDifficulty.Facil,
            "Media" => MissionDifficulty.Media,
            "Dificil" => MissionDifficulty.Dificil,
            _ => throw new ArgumentException($"Invalid difficulty: {request.Difficulty}", nameof(request.Difficulty))
        };

        var type = request.Type switch
        {
            "Tesoro" => MissionType.Tesoro,
            "Trivia" => MissionType.Trivia,
            _ => throw new ArgumentException("Invalid mission type. Must be 'Tesoro' or 'Trivia'.", nameof(request.Type))
        };

        var mission = new Mission(
            Guid.NewGuid(),
            title,
            request.Description,
            difficulty,
            request.TimeMinutes,
            type);

        await _missionRepository.AddAsync(mission);

        return new CreateMissionResponse(mission.Id);
    }
}
