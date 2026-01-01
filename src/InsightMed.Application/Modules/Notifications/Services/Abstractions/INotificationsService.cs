using InsightMed.Application.Modules.Notifications.Enums;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.Notifications.Services.Abstractions;

public interface INotificationsService
{
    Task<List<Notification>> GetAllAsync(NotificationFilter filter);
    Task AddAsync(Notification notification);
    Task DeleteAllAsync();
    Task MarkAsSeenAsync(List<int> ids);
    Task<bool> HasUnseenAsync();
}