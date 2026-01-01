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

    public async Task<List<Notification>> GetAllAsync(NotificationFilter filter)
    {
        var query = _context.Notifications.AsNoTracking();

        query = filter switch
        {
            NotificationFilter.Seen => query.Where(notification => notification.Seen),
            NotificationFilter.Unseen => query.Where(notification => !notification.Seen),
            _ => query
        };

        return await query
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

    public async Task MarkAsSeenAsync(List<int> ids)
    {
        await _context.Notifications
            .Where(notification => ids.Contains(notification.Id))
            .ExecuteUpdateAsync(x => x.SetProperty(notification => notification.Seen, true))
            .ConfigureAwait(false);
    }
}