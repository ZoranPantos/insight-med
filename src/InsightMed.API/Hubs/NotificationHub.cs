using InsightMed.Application.Modules.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace InsightMed.API.Hubs;

public sealed class NotificationHub : Hub<INotificationClient>
{
    private readonly ISender _sender;

    public NotificationHub(ISender sender) =>
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

    public async Task CheckUnseen()
    {
        // TODO: Consider throwing exception if userId is null or empty instead of returning, and not use default value
        string? userId = Context.UserIdentifier ?? string.Empty;
        if (string.IsNullOrEmpty(userId)) return;

        bool hasUnseen = await _sender.Send(new HasUnseenNotificationsQuery(userId));
        await Clients.Caller.ReceiveUnseenStatus(hasUnseen);
    }
}

public interface INotificationClient
{
    Task ReceiveUnseenStatus(bool hasUnseen);
}