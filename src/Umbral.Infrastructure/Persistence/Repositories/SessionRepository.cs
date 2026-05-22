using Microsoft.EntityFrameworkCore;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
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
            .Include(s => s.Mission)
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
}
