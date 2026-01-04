namespace InsightMed.Application.Modules.Notifications.Services.Abstractions;

public interface INotificationsNotifierService
{
    Task NotifyUnseenStatusAsync(string userId, bool hasUnseen);
}