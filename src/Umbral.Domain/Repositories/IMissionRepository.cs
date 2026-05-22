using Umbral.Domain.Entities;
using Umbral.Domain.Primitives;
using Umbral.Domain.ValueObjects;

namespace Umbral.Domain.Repositories;

public interface IMissionRepository
{
    Task<Mission?> GetByIdAsync(Guid id);
    Task<bool> ExistsByTitleAsync(MissionTitle title);
    Task AddAsync(Mission mission);

    /// <summary>
    /// Returns a paginated, filterable list of missions.
    /// Filters: difficulty, status (exact match), search (partial match on title, case-insensitive).
    /// </summary>
    Task<PaginatedResult<Mission>> GetAllAsync(
        int page,
        int pageSize,
        string? difficulty = null,
        string? status = null,
        string? search = null);
}
