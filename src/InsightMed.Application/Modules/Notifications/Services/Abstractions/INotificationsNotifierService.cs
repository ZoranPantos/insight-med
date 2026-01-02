namespace InsightMed.Application.Modules.Notifications.Services.Abstractions;

public interface INotificationsNotifierService
{
    Task NotifyUnseenStatusAsync(bool hasUnseen);
}