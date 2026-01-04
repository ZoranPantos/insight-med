using InsightMed.API.Hubs;
using InsightMed.Application.Modules.Notifications.Services.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace InsightMed.API.Services;

public class NotificationsNotifierService : INotificationsNotifierService
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

    public NotificationsNotifierService(IHubContext<NotificationHub, INotificationClient> hubContext) =>
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    public async Task NotifyUnseenStatusAsync(string userId, bool hasUnseen)
    {
        // TODO: Throw exception here instead of return?
        if (string.IsNullOrEmpty(userId)) return;

        await _hubContext.Clients.User(userId).ReceiveUnseenStatus(hasUnseen);
    }
}