using InsightMed.Application.Modules.Notifications.Enums;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.Notifications.Services.Abstractions;

public interface INotificationsService
{
    Task<List<Notification>> GetAllAsync(string userId, NotificationFilter filter);
    Task AddAsync(Notification notification);
    Task DeleteAllAsync();
    Task MarkAsSeenAsync(string userId, List<int> ids);
    Task<bool> HasUnseenAsync(string userId);
}