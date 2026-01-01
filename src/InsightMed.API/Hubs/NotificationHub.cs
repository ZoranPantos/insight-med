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
        bool hasUnseen = await _sender.Send(new HasUnseenNotificationsQuery());
        await Clients.Caller.ReceiveUnseenStatus(hasUnseen);
    }
}

public interface INotificationClient
{
    Task ReceiveUnseenStatus(bool hasUnseen);
}