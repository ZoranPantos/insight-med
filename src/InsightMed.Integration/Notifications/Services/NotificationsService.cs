using InsightMed.Application.Notifications.Services.Abstractions;
using InsightMed.Domain.Entities;
using InsightMed.Integration.Data;

namespace InsightMed.Infrastructure.Notifications.Services;

public sealed class NotificationsService : INotificationsService
{
    private readonly AppDbContext _context;

    public NotificationsService(AppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task AddAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
}
