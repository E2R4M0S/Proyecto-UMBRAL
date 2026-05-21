using Microsoft.EntityFrameworkCore;
using Umbral.Domain.Entities;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Infrastructure.Persistence.Repositories;

public class MissionRepository : IMissionRepository
{
    private readonly UmbralDbContext _context;

    public MissionRepository(UmbralDbContext context)
    {
        _context = context;
    }

    public async Task<Mission?> GetByIdAsync(Guid id)
    {
        return await _context.Missions.FindAsync(id);
    }

    public async Task<bool> ExistsByTitleAsync(MissionTitle title)
    {
        return await _context.Missions
            .AnyAsync(m => m.Title == title);
    }

    public async Task AddAsync(Mission mission)
    {
        await _context.Missions.AddAsync(mission);
        await _context.SaveChangesAsync();
    }
}
