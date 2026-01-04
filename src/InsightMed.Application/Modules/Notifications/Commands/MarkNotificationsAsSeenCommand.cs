using InsightMed.Application.Modules.Notifications.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.Modules.Notifications.Commands;

public sealed record MarkNotificationsAsSeenCommand(string UserId, List<int> Ids) : IRequest;

public class MarkNotificationsAsSeenCommandHandler : IRequestHandler<MarkNotificationsAsSeenCommand>
{
    private readonly INotificationsService _notificationsService;

    public MarkNotificationsAsSeenCommandHandler(INotificationsService notificationService) =>
        _notificationsService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

    public async Task Handle(MarkNotificationsAsSeenCommand request, CancellationToken cancellationToken) =>
        await _notificationsService.MarkAsSeenAsync(request.UserId, request.Ids);
}
