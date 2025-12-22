using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Notifications.Services.Abstractions;
using InsightMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.Notifications.Services;

public sealed class NotificationsService : INotificationsService
{
    private readonly IAppDbContext _context;

    public NotificationsService(IAppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<List<Notification>> GetAllAsync()
    {
        return await _context.Notifications
            .AsNoTracking()
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
}
