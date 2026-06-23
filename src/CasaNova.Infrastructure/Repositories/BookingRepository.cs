using CasaNova.Domain.Entities;
using CasaNova.Domain.Interfaces;
using CasaNova.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CasaNova.Infrastructure.Repositories;

public class BookingRepository : BaseRepository<Booking>, IBookingRepository
{
    public BookingRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Booking>> GetByUserAsync(Guid userId, CancellationToken ct = default)
        => await _dbSet
            .Include(b => b.Property)
                .ThenInclude(p => p.Photos)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Booking>> GetByPropertyAsync(Guid propertyId, CancellationToken ct = default)
        => await _dbSet
            .Include(b => b.User)
            .Where(b => b.PropertyId == propertyId)
            .OrderByDescending(b => b.CheckIn)
            .ToListAsync(ct);

    public async Task<IEnumerable<Booking>> GetByOwnerAsync(Guid ownerId, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var query = _dbSet
            .Include(b => b.Property)
            .Include(b => b.User)
            .Where(b => b.Property.OwnerId == ownerId);

        if (from.HasValue)
            query = query.Where(b => b.CheckIn >= from.Value);

        if (to.HasValue)
            query = query.Where(b => b.CheckOut <= to.Value);

        return await query.OrderByDescending(b => b.CheckIn).ToListAsync(ct);
    }

    public async Task<bool> HasOverlapAsync(Guid propertyId, DateTime checkIn, DateTime checkOut, Guid? excludeBookingId = null, CancellationToken ct = default)
    {
        var query = _dbSet.Where(b =>
            b.PropertyId == propertyId &&
            b.Status != Domain.Enums.BookingStatus.Cancelled &&
            b.CheckIn < checkOut &&
            b.CheckOut > checkIn);

        if (excludeBookingId.HasValue)
            query = query.Where(b => b.Id != excludeBookingId.Value);

        return await query.AnyAsync(ct);
    }
}