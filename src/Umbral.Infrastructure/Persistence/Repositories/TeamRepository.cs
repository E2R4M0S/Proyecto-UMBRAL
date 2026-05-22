using Microsoft.EntityFrameworkCore;
using Umbral.Domain.Entities;
using Umbral.Domain.Repositories;

namespace Umbral.Infrastructure.Persistence.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly UmbralDbContext _context;

    public TeamRepository(UmbralDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Team team)
    {
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await _context.Teams
            .Include(t => t.Members)
            .Include(t => t.Leader)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Teams
            .AnyAsync(t => EF.Functions.ILike(t.Name.Value, name));
    }
}
