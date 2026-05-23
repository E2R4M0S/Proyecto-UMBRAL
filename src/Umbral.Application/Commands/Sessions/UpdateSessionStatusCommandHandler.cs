using MediatR;
using Umbral.Application.Common.Exceptions;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;

namespace Umbral.Application.Commands.Sessions;

public class UpdateSessionStatusCommandHandler : IRequestHandler<UpdateSessionStatusCommand>
{
    private readonly ISessionRepository _repository;

    public UpdateSessionStatusCommandHandler(ISessionRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateSessionStatusCommand request, CancellationToken cancellationToken)
    {
        var session = await _repository.GetByIdAsync(request.SessionId);
        if (session is null)
            throw new NotFoundException($"Session with ID '{request.SessionId}' not found.");

        var newStatus = Enum.Parse<SessionStatus>(request.Status);

        // Map status string to domain method
        switch (newStatus)
        {
            case SessionStatus.EnPreparacion:
                session.MoveToPreparing();
                break;
            case SessionStatus.Activa:
                session.Activate();
                break;
            case SessionStatus.Pausada:
                session.Pause();
                break;
            case SessionStatus.Finalizada:
                session.Finalize();
                break;
            case SessionStatus.Cancelada:
                session.Cancel();
                break;
            default:
                throw new InvalidOperationException($"Cannot transition to {newStatus}.");
        }

        await _repository.UpdateAsync(session);
    }
}
