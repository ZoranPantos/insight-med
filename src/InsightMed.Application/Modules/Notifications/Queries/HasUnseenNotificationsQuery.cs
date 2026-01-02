using InsightMed.Application.Modules.Notifications.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.Modules.Notifications.Queries;

public sealed record HasUnseenNotificationsQuery : IRequest<bool>;

public sealed class HasUnseenNotificationsQueryHandler : IRequestHandler<HasUnseenNotificationsQuery, bool>
{
    private readonly INotificationsService _notificationsService;

    public HasUnseenNotificationsQueryHandler(INotificationsService notificationsService) =>
        _notificationsService = notificationsService ?? throw new ArgumentNullException(nameof(notificationsService));

    public async Task<bool> Handle(HasUnseenNotificationsQuery request, CancellationToken cancellationToken) =>
        await _notificationsService.HasUnseenAsync();
}