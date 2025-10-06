using AutoMapper;
using InsightMed.Application.LabRequests.Models;
using InsightMed.Application.LabRequests.Services.Abstractions;
using MediatR;

namespace InsightMed.Application.LabRequests.Queries;

public sealed record GetAllLabRequestsQuery : IRequest<GetAllLabRequestsQueryResponse>;

public sealed class GetAllLabRequestsQueryHandler
    : IRequestHandler<GetAllLabRequestsQuery, GetAllLabRequestsQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly ILabRequestsService _labRequestsService;

    public GetAllLabRequestsQueryHandler(IMapper mapper, ILabRequestsService labRequestsService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _labRequestsService = labRequestsService ?? throw new ArgumentNullException(nameof(labRequestsService));
    }

    public async Task<GetAllLabRequestsQueryResponse> Handle(
        GetAllLabRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var labRequests = await _labRequestsService.GetAllAsync();

        var response = new GetAllLabRequestsQueryResponse
        {
            LabRequests = _mapper.Map<List<LabRequestLiteResponse>>(labRequests)
        };

        return response;
    }
}