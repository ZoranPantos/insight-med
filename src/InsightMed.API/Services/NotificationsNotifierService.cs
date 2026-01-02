using InsightMed.API.Hubs;
using InsightMed.Application.Modules.Notifications.Services.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace InsightMed.API.Services;

public class NotificationsNotifierService : INotificationsNotifierService
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

    public NotificationsNotifierService(IHubContext<NotificationHub, INotificationClient> hubContext) =>
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    public async Task NotifyUnseenStatusAsync(bool hasUnseen)
    {
        // We broadcast to "All" connected clients for now
        // TODO: Consider mapping this to a specific User ID (Clients.User(userId))
        await _hubContext.Clients.All.ReceiveUnseenStatus(hasUnseen);
    }
}