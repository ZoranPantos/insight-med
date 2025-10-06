using InsightMed.Application.Notifications.Services.Abstractions;
using InsightMed.Domain.Entities;
using MediatR;

namespace InsightMed.Application.Notifications.Commands;

public sealed record CreateNotificationCommand(string Message, int LabReportId) : IRequest;

public sealed class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand>
{
    private readonly INotificationsService _notificationsService;

    public CreateNotificationCommandHandler(INotificationsService notificationService) =>
        _notificationsService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

    public async Task Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Message = request.Message,
            LabReportId = request.LabReportId
        };

        await _notificationsService.AddAsync(notification);
    }
}