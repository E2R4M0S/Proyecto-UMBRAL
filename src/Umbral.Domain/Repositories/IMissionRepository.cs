using Umbral.Domain.Entities;
using Umbral.Domain.ValueObjects;

namespace Umbral.Domain.Repositories;

public interface IMissionRepository
{
    Task<Mission?> GetByIdAsync(Guid id);
    Task<bool> ExistsByTitleAsync(MissionTitle title);
    Task AddAsync(Mission mission);
}
