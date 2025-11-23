using InsightMed.Application.Common.Abstractions.Messaging;
using InsightMed.Application.LabRequests.Services.Abstractions;
using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;
using MediatR;
using System.Text.Json;

namespace InsightMed.Application.LabRequests.Commands;

public sealed record CreateLabRequestCommand(int PatientId, List<int> LabParameterIds) : IRequest;

public sealed class CreateLabRequestCommandHandler : IRequestHandler<CreateLabRequestCommand>
{
    private readonly ILabRequestsService _labRequestsService;
    private readonly ILabRpcClient _labRpcClient;

    public CreateLabRequestCommandHandler(ILabRequestsService labRequestsService, ILabRpcClient labRpcClient)
    {
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

        string rpcResponse = await _labRpcClient.CallAsync(labRequestJson, cancellationToken);

        // TODO: For testing only
        Console.WriteLine(rpcResponse);
    }
}