using MediatR;
using Umbral.Application.Common.Exceptions;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;

namespace Umbral.Application.Commands.Operators;

public class DeactivateOperatorCommandHandler : IRequestHandler<DeactivateOperatorCommand>
{
    private readonly IUserRepository _repository;

    public DeactivateOperatorCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeactivateOperatorCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.OperatorId);
        if (user is null)
            throw new NotFoundException($"Operator with ID '{request.OperatorId}' not found.");

        if (user.Role != UserRole.Operator)
            throw new InvalidOperationException("User is not an Operator.");

        user.Deactivate();
        await _repository.UpdateAsync(user);
    }
}
