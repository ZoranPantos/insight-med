using InsightMed.Application.Notifications.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.Notifications.Commands;

public record DeleteAllNotificationsCommand : IRequest;

public sealed class DeleteAllNotificationsCommandHandler : IRequestHandler<DeleteAllNotificationsCommand>
{
    private readonly INotificationsService _notificationsService;

    public DeleteAllNotificationsCommandHandler(INotificationsService notificationService) =>
        _notificationsService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

    public async Task Handle(DeleteAllNotificationsCommand request, CancellationToken cancellationToken) =>
        await _notificationsService.DeleteAllAsync();
}