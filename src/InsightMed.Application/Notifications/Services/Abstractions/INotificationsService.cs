using InsightMed.Domain.Entities;

namespace InsightMed.Application.Notifications.Services.Abstractions;

public interface INotificationsService
{
    Task AddAsync(Notification notification);
}
