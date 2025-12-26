using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.Notifications.Services.Abstractions;

public interface INotificationsService
{
    Task<List<Notification>> GetAllAsync();
    Task AddAsync(Notification notification);
    Task DeleteAllAsync();
}
