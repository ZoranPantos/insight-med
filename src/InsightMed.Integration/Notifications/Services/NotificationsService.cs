using InsightMed.Application.Common.Abstractions;
using InsightMed.Application.Notifications.Services.Abstractions;
using InsightMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.Notifications.Services;

public sealed class NotificationsService : INotificationsService
{
    private readonly IAppDbContext _context;

    public NotificationsService(IAppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public Task<List<Notification>> GetAllAsync() => _context.Notifications.ToListAsync();

    public async Task AddAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllAsync() => await _context.Notifications.ExecuteDeleteAsync();
}
