using Microsoft.EntityFrameworkCore;
using Umbral.Domain.Entities;
using Umbral.Domain.Enums;
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

    public async Task<List<User>> GetParticipantsByIdsAsync(List<Guid> ids)
    {
        return await _context.Users
            .Where(u => ids.Contains(u.Id) && u.Role == UserRole.Participant)
            .ToListAsync();
    }
}
