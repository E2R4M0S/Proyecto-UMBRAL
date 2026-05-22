using Umbral.Domain.Entities;

namespace Umbral.Domain.Repositories;

public interface ITeamRepository
{
    Task AddAsync(Team team);
    Task<Team?> GetByIdAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name);
}
