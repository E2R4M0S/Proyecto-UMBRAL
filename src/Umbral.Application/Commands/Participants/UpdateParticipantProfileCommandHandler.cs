using MediatR;
using Umbral.Application.Common.Exceptions;
using Umbral.Application.Common.Interfaces;
using Umbral.Application.DTOs.Participants;
using Umbral.Domain.Enums;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Application.Commands.Participants;

public class UpdateParticipantProfileCommandHandler
    : IRequestHandler<UpdateParticipantProfileCommand, UpdateParticipantProfileResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public UpdateParticipantProfileCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<UpdateParticipantProfileResponse> Handle(
        UpdateParticipantProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null)
            throw new NotFoundException("User not found.");

        if (user.Role != UserRole.Participant)
            throw new InvalidOperationException("Only participants can update their profile.");

        bool tokenNeedsRefresh = false;

        // Update Name if provided
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.UpdateName(request.Name);
            tokenNeedsRefresh = true;
        }

        // Update Alias if provided
        if (!string.IsNullOrWhiteSpace(request.Alias))
        {
            var newAlias = Alias.Create(request.Alias);

            // Check alias uniqueness (excluding self)
            var existingUser = await _userRepository.GetByAliasAsync(newAlias);
            if (existingUser != null && existingUser.Id != request.UserId)
                throw new ConflictException($"Alias '{request.Alias}' is already in use.");

            user.UpdateAlias(newAlias);
        }

        // Change password if provided
        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                throw new InvalidOperationException("Current password is required to set a new password.");

            if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
                throw new InvalidOperationException("Current password is incorrect.");

            user.ChangePassword(_passwordHasher.Hash(request.NewPassword));
            tokenNeedsRefresh = true;
        }

        await _userRepository.UpdateAsync(user);

        // Generate new token if needed
        string? newToken = null;
        DateTime? expiresAt = null;
        if (tokenNeedsRefresh)
        {
            newToken = _tokenService.GenerateToken(user, out var exp);
            expiresAt = exp;
        }

        return new UpdateParticipantProfileResponse(
            UserId: user.Id,
            Name: user.Name,
            Alias: user.Alias,
            Token: newToken,
            ExpiresAt: expiresAt
        );
    }
}
