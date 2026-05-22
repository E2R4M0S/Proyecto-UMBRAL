using Umbral.Domain.Entities;

namespace Umbral.Domain.Repositories;

public interface ISessionRepository
{
    Task AddAsync(Session session);
    Task<Session?> GetByIdAsync(Guid id);
    Task<bool> ExistsByNameWithActiveStatusAsync(string name);
    Task<bool> IsPinUniqueAsync(string pin);
}
