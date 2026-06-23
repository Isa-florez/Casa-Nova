using CasaNova.Domain.Entities;
using CasaNova.Domain.Interfaces;
using CasaNova.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CasaNova.Infrastructure.Repositories;

public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Notification>> GetByUserAsync(Guid userId, CancellationToken ct = default)
        => await _dbSet
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Notification>> GetUnsentEmailsAsync(CancellationToken ct = default)
        => await _dbSet
            .Include(n => n.User)
            .Where(n => !n.EmailSent)
            .ToListAsync(ct);
}