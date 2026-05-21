using MediatR;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.Common.Interfaces;
using Umbral.Application.DTOs.Operators;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Application.Commands.Operators;

public class CreateOperatorCommandHandler : IRequestHandler<CreateOperatorCommand, CreateOperatorResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateOperatorCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<CreateOperatorResponse> Handle(CreateOperatorCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);

        var exists = await _userRepository.ExistsByEmailAsync(email);
        if (exists)
            throw new ConflictException("Email already exists.");

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User(
            Guid.NewGuid(),
            request.Name,
            email,
            passwordHash,
            UserRole.Operator);

        await _userRepository.AddAsync(user);

        return new CreateOperatorResponse(user.Id);
    }
}
