using InsightMed.Application.Auth.Services.Abstractions;
using InsightMed.Application.Common.Abstractions.Messaging;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.LabReports.Models;
using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using InsightMed.Application.Modules.LabRequests.Services.Abstractions;
using InsightMed.Application.Modules.Notifications.Services.Abstractions;
using InsightMed.Application.Modules.Patients.Services.Abstractions;
using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InsightMed.Application.Modules.LabRequests.Commands;

public sealed record CreateLabRequestCommand(int PatientId, List<int> LabParameterIds) : IRequest;

public sealed class CreateLabRequestCommandHandler : IRequestHandler<CreateLabRequestCommand>
{
    private readonly ILogger<CreateLabRequestCommandHandler> _logger;
    private readonly ILabRequestsService _labRequestsService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILabRpcClient _labRpcClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CreateLabRequestCommandHandler(
        ILogger<CreateLabRequestCommandHandler> logger,
        ILabRequestsService labRequestsService,
        ICurrentUserService currentUserService,
        ILabRpcClient labRpcClient,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _labRequestsService = labRequestsService ?? throw new ArgumentNullException(nameof(labRequestsService));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _labRpcClient = labRpcClient ?? throw new ArgumentNullException(nameof(labRpcClient));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    /// <summary>
    /// Besides creating and storing a lab request object, handler initiates fire-and-forget background task for processing RPC response asynchronously
    /// with ProcessLabRpcResponseAsync method, and completes without waiting on it.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(CreateLabRequestCommand request, CancellationToken cancellationToken)
    {
        string? userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId)) throw new UnauthorizedException("User not found");

        var labRequest = new LabRequest
        {
            PatientId = request.PatientId,
            LabParameterIds = request.LabParameterIds,
            Created = DateTime.UtcNow,
            LabRequestState = LabRequestState.Pending
        };

        string labRequestJson = JsonSerializer.Serialize(labRequest);

        await _labRequestsService.AddAsync(labRequest);

        _ = Task.Run(() => ProcessLabRpcResponseAsync(labRequestJson, labRequest.Id, labRequest.PatientId, userId), CancellationToken.None);
    }

    /// <summary>
    /// Fire-and-forget background task that processes the RPC response asynchronously.
    /// Exceptions must be handled here as they won't be caught by global exception handlers or middleware since the request has already completed.
    /// </summary>
    /// <param name="labRequestJson"></param>
    /// <param name="labRequest"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task ProcessLabRpcResponseAsync(string labRequestJson, int labRequestId, int patientId, string userId)
    {
        try
        {
            string rpcResponse = await _labRpcClient
                .CallAsync(labRequestJson, CancellationToken.None)
                .ConfigureAwait(false);

            string rpcResponseTransformed = rpcResponse.Replace("{\"LabParameterValueResponseDtos\":", string.Empty);
            rpcResponseTransformed = rpcResponseTransformed[..^1];

            var labReportRpcResponse = JsonSerializer.Deserialize<LabReportRpcServerResponse>(rpcResponse) ??
                throw new InvalidOperationException("Failed to deserialize Lab Report RPC Server response");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var labReportsService = scope.ServiceProvider.GetRequiredService<ILabReportsService>();
                var labRequestsService = scope.ServiceProvider.GetRequiredService<ILabRequestsService>();
                var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationsService>();
                var notifierService = scope.ServiceProvider.GetRequiredService<INotificationsNotifierService>();
                var patientsService = scope.ServiceProvider.GetRequiredService<IPatientsService>();

                var labReport = new LabReport
                {
                    Content = rpcResponseTransformed,
                    Created = DateTime.UtcNow,
                    LabRequestId = labRequestId,
                    PatientId = patientId
                };

                await labReportsService.AddAsync(labReport);

                var patient = await patientsService.GetByIdAsync(patientId)
                    ?? throw new ResourceNotFoundException($"Patient with ID {patientId} not found");

                var notification = new Notification
                {
                    LabReportId = labReport.Id,
                    RequesterId = userId,
                    Message = $"Report for patient {patient.FirstName} {patient.LastName} {patient.Uid} is available. Date created UTC: {labReport.Created}"
                };

                await notificationsService.AddAsync(notification);
                await notifierService.NotifyUnseenStatusAsync(userId, true);

                _ = await labRequestsService.SetStateAsync(labReport.LabRequestId.Value, LabRequestState.Completed)
                    ?? throw new ResourceNotFoundException($"Lab request with ID {labReport.LabRequestId.Value} not found");
            }

            _logger.LogInformation("Background RPC response for {RequestName} (LabRequestId={LabRequestId}): {RpcResponse}",
                nameof(CreateLabRequestCommand),
                labRequestId,
                rpcResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Background RPC error for {RequestName} (LabRequestId={LabRequestId})",
                nameof(CreateLabRequestCommand),
                labRequestId);
        }
    }
}