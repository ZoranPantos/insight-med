using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Modules.Notifications.Enums;
using InsightMed.Application.Modules.Notifications.Services.Abstractions;
using InsightMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.Modules.Notifications.Services;

public sealed class NotificationsService : INotificationsService
{
    private readonly IAppDbContext _context;

    public NotificationsService(IAppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<List<Notification>> GetAllAsync(string userId, NotificationFilter filter)
    {
        var query = _context.Notifications
            .AsNoTracking()
            .Where(notification => notification.RequesterId.Equals(userId));

        query = filter switch
        {
            NotificationFilter.Seen => query.Where(notification => notification.Seen),
            NotificationFilter.Unseen => query.Where(notification => !notification.Seen),
            _ => query
        };

        return await query
            .OrderByDescending(notification => notification.Id)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task AddAsync(Notification notification)
    {
        _context.Notifications.Add(notification);

        await _context
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Notifications
            .ExecuteDeleteAsync()
            .ConfigureAwait(false);
    }

    public async Task MarkAsSeenAsync(string userId, List<int> ids)
    {
        await _context.Notifications
            .Where(notification => ids.Contains(notification.Id) && notification.RequesterId.Equals(userId))
            .ExecuteUpdateAsync(x => x.SetProperty(notification => notification.Seen, true))
            .ConfigureAwait(false);
    }

    public async Task<bool> HasUnseenAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.RequesterId.Equals(userId))
            .AnyAsync(notification => !notification.Seen)
            .ConfigureAwait(false);
    }
}