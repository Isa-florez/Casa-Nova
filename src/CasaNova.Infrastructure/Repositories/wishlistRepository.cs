using CasaNova.Domain.Entities;
using CasaNova.Domain.Interfaces;
using CasaNova.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CasaNova.Infrastructure.Repositories;

public class WishlistRepository : BaseRepository<Wishlist>, IWishlistRepository
{
    public WishlistRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Wishlist>> GetByUserAsync(Guid userId, CancellationToken ct = default)
        => await _dbSet
            .Include(w => w.Property)
                .ThenInclude(p => p.Photos)
            .Where(w => w.UserId == userId)
            .ToListAsync(ct);

    public async Task<Wishlist?> GetByUserAndPropertyAsync(Guid userId, Guid propertyId, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(w => w.UserId == userId && w.PropertyId == propertyId, ct);
}