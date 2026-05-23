using Umbral.Domain.Entities;
using Umbral.Domain.ValueObjects;

namespace Umbral.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(Email email);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByIdWithTrackingAsync(Guid id);
    Task<User?> GetByAliasAsync(Alias alias);
    Task<bool> ExistsByEmailAsync(Email email);
    Task<bool> ExistsByAliasAsync(Alias alias);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<List<User>> GetParticipantsByIdsAsync(List<Guid> ids);
}
