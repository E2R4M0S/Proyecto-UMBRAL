using Umbral.Domain.Entities;

namespace Umbral.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user, out DateTime expiresAt);
}
