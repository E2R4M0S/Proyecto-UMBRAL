using MediatR;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.Common.Interfaces;
using Umbral.Application.DTOs.Auth;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Application.Commands.Auth;

public class RegisterParticipantCommandHandler : IRequestHandler<RegisterParticipantCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public RegisterParticipantCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(RegisterParticipantCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);
        var alias = Alias.Create(request.Alias);

        var emailExists = await _userRepository.ExistsByEmailAsync(email);
        if (emailExists)
            throw new ConflictException("Email already exists.");

        var aliasExists = await _userRepository.ExistsByAliasAsync(alias);
        if (aliasExists)
            throw new ConflictException("Alias already exists.");

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User(
            Guid.NewGuid(),
            request.Name,
            email,
            passwordHash,
            UserRole.Participant,
            alias);

        await _userRepository.AddAsync(user);

        var token = _tokenService.GenerateToken(user, out var expiresAt);

        return new LoginResponse(
            Token: token,
            ExpiresAt: expiresAt,
            UserId: user.Id,
            UserName: user.Name,
            Email: user.Email.ToString(),
            Role: user.Role.ToString()
        );
    }
}
