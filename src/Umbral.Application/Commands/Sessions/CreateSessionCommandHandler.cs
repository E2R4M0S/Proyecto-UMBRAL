using MediatR;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.DTOs.Sessions;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;

namespace Umbral.Application.Commands.Sessions;

public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, CreateSessionResponse>
{
    private readonly IMissionRepository _missionRepository;
    private readonly ISessionRepository _sessionRepository;

    public CreateSessionCommandHandler(IMissionRepository missionRepository, ISessionRepository sessionRepository)
    {
        _missionRepository = missionRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<CreateSessionResponse> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Mission exists and is Activa
        var mission = await _missionRepository.GetByIdAsync(request.MissionId);
        if (mission is null)
            throw new NotFoundException($"Mission with ID {request.MissionId} not found.");

        if (mission.Status != MissionStatus.Activa)
            throw new ArgumentException("Mission must be in Activa status.", nameof(request.MissionId));

        // 2. Check name uniqueness (no other session with same name AND status in {Activa, Pausada, EnPreparacion, Programada})
        var nameExists = await _sessionRepository.ExistsByNameWithActiveStatusAsync(request.Name.Trim());
        if (nameExists)
            throw new ConflictException($"A session with the name '{request.Name.Trim()}' is already active or pending.");

        // 3. Generate unique PIN
        var pin = await GenerateUniquePinAsync();

        // 4. Create Session
        var session = new Session(Guid.NewGuid(), request.Name.Trim(), request.MissionId, pin);

        // 5. Persist
        await _sessionRepository.AddAsync(session);

        return new CreateSessionResponse(session.Id, session.Pin);
    }

    private async Task<string> GenerateUniquePinAsync()
    {
        for (int i = 0; i < 10; i++)
        {
            var pin = Random.Shared.Next(0, 1_000_000).ToString("D6");
            if (await _sessionRepository.IsPinUniqueAsync(pin))
                return pin;
        }
        throw new Exception("Unable to generate a unique PIN after 10 attempts.");
    }
}
