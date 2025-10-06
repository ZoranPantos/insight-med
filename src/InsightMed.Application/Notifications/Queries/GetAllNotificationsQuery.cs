using AutoMapper;
using InsightMed.Application.Notifications.Models;
using InsightMed.Application.Notifications.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.Notifications.Queries;

public sealed record GetAllNotificationsQuery : IRequest<GetAllNotificationsQueryResponse>;

public sealed class GetAllNotificationsQueryHandler : IRequestHandler<GetAllNotificationsQuery, GetAllNotificationsQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly INotificationsService _notificationsService;

    public GetAllNotificationsQueryHandler(IMapper mapper, INotificationsService notificationsService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _notificationsService = notificationsService ?? throw new ArgumentNullException(nameof(notificationsService));
    }

    public async Task<GetAllNotificationsQueryResponse> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _notificationsService.GetAllAsync();

        var response = new GetAllNotificationsQueryResponse
        {
            Notifications = _mapper.Map<List<NotificationLiteResponse>>(notifications)
        };

        return response;
    }
}