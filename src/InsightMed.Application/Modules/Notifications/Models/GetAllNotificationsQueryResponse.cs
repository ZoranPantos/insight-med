namespace InsightMed.Application.Modules.Notifications.Models;

public sealed class GetAllNotificationsQueryResponse
{
    public List<NotificationLiteResponse> Notifications { get; set; } = [];
}
