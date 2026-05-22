using Microsoft.EntityFrameworkCore;
using Umbral.Domain.Entities;
using Umbral.Domain.Primitives;
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

    public async Task<PaginatedResult<Mission>> GetAllAsync(
        int page,
        int pageSize,
        string? difficulty = null,
        string? status = null,
        string? search = null)
    {
        // Clamp pagination
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        IQueryable<Mission> query = _context.Missions;

        // Filter composition
        if (!string.IsNullOrWhiteSpace(difficulty))
            query = query.Where(m => m.Difficulty.ToString() == difficulty);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(m => m.Status.ToString() == status);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(m => m.Title.Value.ToLower().Contains(search.ToLower()));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Mission>(items, page, pageSize, totalCount);
    }
}
