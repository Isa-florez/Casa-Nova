using CasaNova.Domain.Interfaces;
using CasaNova.Infrastructure.Persistence;

namespace CasaNova.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IPropertyRepository Properties { get; }
    public IBookingRepository Bookings { get; }
    public IUserRepository Users { get; }
    public IWishlistRepository Wishlists { get; }
    public INotificationRepository Notifications { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Properties = new PropertyRepository(context);
        Bookings = new BookingRepository(context);
        Users = new UserRepository(context);
        Wishlists = new WishlistRepository(context);
        Notifications = new NotificationRepository(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}