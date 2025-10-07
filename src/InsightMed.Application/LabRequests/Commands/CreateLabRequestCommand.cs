using InsightMed.Application.LabRequests.Services.Abstractions;
using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;
using MediatR;

namespace InsightMed.Application.LabRequests.Commands;

public sealed record CreateLabRequestCommand(int PatientId, List<int> LabParameterIds) : IRequest;

public sealed class CreateLabRequestCommandHandler : IRequestHandler<CreateLabRequestCommand>
{
    private readonly ILabRequestsService _labRequestsService;

    public CreateLabRequestCommandHandler(ILabRequestsService labRequestsService) =>
        _labRequestsService = labRequestsService ?? throw new ArgumentNullException(nameof(labRequestsService));

    public async Task Handle(CreateLabRequestCommand request, CancellationToken cancellationToken)
    {
        var labRequest = new LabRequest
        {
            PatientId = request.PatientId,
            LabParameterIds = request.LabParameterIds,
            Created = DateTime.UtcNow,
            LabRequestState = LabRequestState.Pending
        };

        await _labRequestsService.AddAsync(labRequest);

        // TODO: Add functionality and logic for sending this request to the Lab RPC Server via RabbitMQ
    }
}