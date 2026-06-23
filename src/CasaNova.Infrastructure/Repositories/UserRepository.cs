using CasaNova.Domain.Entities;
using CasaNova.Domain.Interfaces;
using CasaNova.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CasaNova.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);
}