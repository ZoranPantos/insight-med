using InsightMed.Application.Common.Abstractions.Messaging;
using InsightMed.Application.LabRequests.Services.Abstractions;
using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InsightMed.Application.LabRequests.Commands;

public sealed record CreateLabRequestCommand(int PatientId, List<int> LabParameterIds) : IRequest;

public sealed class CreateLabRequestCommandHandler : IRequestHandler<CreateLabRequestCommand>
{
    private readonly ILogger<CreateLabRequestCommandHandler> _logger;
    private readonly ILabRequestsService _labRequestsService;
    private readonly ILabRpcClient _labRpcClient;

    public CreateLabRequestCommandHandler(
        ILogger<CreateLabRequestCommandHandler> logger,
        ILabRequestsService labRequestsService,
        ILabRpcClient labRpcClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _labRequestsService = labRequestsService ?? throw new ArgumentNullException(nameof(labRequestsService));
        _labRpcClient = labRpcClient ?? throw new ArgumentNullException(nameof(labRpcClient));
    }

    public async Task Handle(CreateLabRequestCommand request, CancellationToken cancellationToken)
    {
        var labRequest = new LabRequest
        {
            PatientId = request.PatientId,
            LabParameterIds = request.LabParameterIds,
            Created = DateTime.UtcNow,
            LabRequestState = LabRequestState.Pending
        };

        string labRequestJson = JsonSerializer.Serialize(labRequest);

        await _labRequestsService.AddAsync(labRequest);

        // This is a detached background task, exceptions need to be handled here because
        // global exception handler will not catch them

        // TODO: Performance of this probably cannot be measured and logged along with correlation ID thorugh the
        // pipeline behavior. Review and analyze this
        _ = Task.Run(async () =>
        {
            try
            {
                string rpcResponse = await _labRpcClient
                    .CallAsync(labRequestJson, CancellationToken.None)
                    .ConfigureAwait(false);

                Console.WriteLine(rpcResponse);

                _logger.LogInformation(
                    "Background RPC response for {RequestName} (LabRequestId={LabRequestId}): {RpcResponse}",
                    nameof(CreateLabRequestCommand),
                    labRequest.Id,
                    rpcResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Background RPC error for {RequestName} (LabRequestId={LabRequestId})",
                    nameof(CreateLabRequestCommand),
                    labRequest.Id);

                Console.WriteLine($"Background RPC error: {ex}");
            }
        }, CancellationToken.None);
    }
}