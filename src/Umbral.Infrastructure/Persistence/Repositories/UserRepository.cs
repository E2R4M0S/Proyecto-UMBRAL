using Microsoft.EntityFrameworkCore;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
using Umbral.Domain.Primitives;
using Umbral.Domain.Repositories;
using Umbral.Domain.ValueObjects;

namespace Umbral.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UmbralDbContext _context;

    public UserRepository(UmbralDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(Email email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByIdWithTrackingAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByAliasAsync(Alias alias)
    {
        var aliasValue = alias.Value;
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Alias != null && u.Alias == aliasValue);
    }

    public async Task<bool> ExistsByEmailAsync(Email email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByAliasAsync(Alias alias)
    {
        var aliasValue = alias.Value;
        return await _context.Users
            .AnyAsync(u => u.Alias != null && u.Alias == aliasValue);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<PaginatedResult<User>> GetAllAsync(int page, int pageSize, string? role = null, string? status = null, string? search = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        IQueryable<User> query = _context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(role))
            query = query.Where(u => u.Role.ToString() == role);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(u => u.Status.ToString() == status);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.Name.Contains(search) || u.Email.Value.Contains(search));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<User>(items, page, pageSize, totalCount);
    }

    public async Task<List<User>> GetParticipantsByIdsAsync(List<Guid> ids)
    {
        return await _context.Users
            .Where(u => ids.Contains(u.Id) && u.Role == UserRole.Participant)
            .ToListAsync();
    }
}
