using Umbral.Domain.Entities;
using Umbral.Domain.Primitives;

namespace Umbral.Domain.Repositories;

public interface ISessionRepository
{
    Task AddAsync(Session session);
    Task<Session?> GetByIdAsync(Guid id);
    Task<bool> ExistsByNameWithActiveStatusAsync(string name);
    Task<bool> IsPinUniqueAsync(string pin);

    /// <summary>
    /// Returns a paginated, filterable list of sessions ordered by StartedAt descending (nulls last).
    /// </summary>
    Task<PaginatedResult<Session>> GetAllAsync(GetSessionsFilter filter, int page, int pageSize);

    /// <summary>
    /// Returns all sessions with status Activa or EnPreparacion, ordered by StartedAt descending.
    /// </summary>
    Task<List<Session>> GetActiveAsync();
}
