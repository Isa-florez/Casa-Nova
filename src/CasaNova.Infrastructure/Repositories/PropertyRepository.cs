using CasaNova.Domain.Entities;
using CasaNova.Domain.Interfaces;
using CasaNova.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CasaNova.Infrastructure.Repositories;

public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
{
    public PropertyRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Property>> SearchAsync(string? city, DateTime? checkIn, DateTime? checkOut, CancellationToken ct = default)
    {
        var query = _dbSet
            .Include(p => p.Photos)
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(p => p.City.ToLower().Contains(city.ToLower()));

        if (checkIn.HasValue && checkOut.HasValue)
            query = query.Where(p => !p.Bookings.Any(b =>
                b.Status != Domain.Enums.BookingStatus.Cancelled &&
                b.CheckIn < checkOut.Value &&
                b.CheckOut > checkIn.Value));

        return await query.ToListAsync(ct);
    }

    public async Task<Property?> GetWithBookingsAsync(Guid id, CancellationToken ct = default)
        => await _dbSet
            .Include(p => p.Bookings)
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<Property>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default)
        => await _dbSet
            .Include(p => p.Photos)
            .Where(p => p.OwnerId == ownerId)
            .ToListAsync(ct);
}