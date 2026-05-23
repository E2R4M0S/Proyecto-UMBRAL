using Microsoft.EntityFrameworkCore;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Primitives;
using Umbral.Domain.Repositories;

namespace Umbral.Infrastructure.Persistence.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly UmbralDbContext _context;

    public SessionRepository(UmbralDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Session session)
    {
        await _context.Sessions.AddAsync(session);
        await _context.SaveChangesAsync();
    }

    public async Task<Session?> GetByIdAsync(Guid id)
    {
        return await _context.Sessions
            .AsNoTracking()
            .Include(s => s.Mission)
            .Include(s => s.SessionTeams)
                .ThenInclude(st => st.Team)
            .AsSplitQuery()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> ExistsByNameWithActiveStatusAsync(string name)
    {
        var activeStatuses = new[]
        {
            SessionStatus.Programada,
            SessionStatus.EnPreparacion,
            SessionStatus.Activa,
            SessionStatus.Pausada
        };

        return await _context.Sessions
            .AnyAsync(s => s.Name == name && activeStatuses.Contains(s.Status));
    }

    public async Task<bool> IsPinUniqueAsync(string pin)
    {
        return !await _context.Sessions.AnyAsync(s => s.Pin == pin);
    }

    public async Task<PaginatedResult<Session>> GetAllAsync(GetSessionsFilter filter, int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        IQueryable<Session> query = _context.Sessions.AsNoTracking();

        if (filter.MissionId.HasValue)
            query = query.Where(s => s.MissionId == filter.MissionId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(s => s.Status.ToString() == filter.Status);

        if (filter.FromDate.HasValue)
            query = query.Where(s => s.StartedAt >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(s => s.StartedAt <= filter.ToDate.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.StartedAt ?? DateTime.MinValue)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Session>(items, page, pageSize, totalCount);
    }

    public async Task<List<Session>> GetActiveAsync()
    {
        var activeStatuses = new[] { SessionStatus.Activa, SessionStatus.EnPreparacion };

        return await _context.Sessions
            .AsNoTracking()
            .Where(s => activeStatuses.Contains(s.Status))
            .OrderByDescending(s => s.StartedAt ?? DateTime.MinValue)
            .ToListAsync();
    }
}
