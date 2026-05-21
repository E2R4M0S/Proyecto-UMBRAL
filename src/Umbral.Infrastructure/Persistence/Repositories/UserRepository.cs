using Microsoft.EntityFrameworkCore;
using Umbral.Domain.Entities;
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
        var emailValue = email.Value;
        return await _context.Users
            .FirstOrDefaultAsync(u => EF.Property<string>(u, "Email") == emailValue);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByAliasAsync(Alias alias)
    {
        var aliasValue = alias.Value;
        return await _context.Users
            .FirstOrDefaultAsync(u => EF.Property<string>(u, "Alias") == aliasValue);
    }

    public async Task<bool> ExistsByEmailAsync(Email email)
    {
        var emailValue = email.Value;
        return await _context.Users
            .AnyAsync(u => EF.Property<string>(u, "Email") == emailValue);
    }

    public async Task<bool> ExistsByAliasAsync(Alias alias)
    {
        var aliasValue = alias.Value;
        return await _context.Users
            .AnyAsync(u => EF.Property<string>(u, "Alias") == aliasValue);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }
}
