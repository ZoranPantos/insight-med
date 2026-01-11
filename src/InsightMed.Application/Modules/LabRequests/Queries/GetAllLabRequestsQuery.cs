using AutoMapper;
using InsightMed.Application.Modules.LabParameters.Services.Abstractions;
using InsightMed.Application.Modules.LabRequests.Models;
using InsightMed.Application.Modules.LabRequests.Services.Abstractions;
using InsightMed.Domain.Entities;
using MediatR;

namespace InsightMed.Application.Modules.LabRequests.Queries;

public sealed record GetAllLabRequestsQuery(string? SearchKey) : IRequest<GetAllLabRequestsQueryResponse>;

public sealed class GetAllLabRequestsQueryHandler
    : IRequestHandler<GetAllLabRequestsQuery, GetAllLabRequestsQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly ILabRequestsService _labRequestsService;
    private readonly ILabParametersService _labParametersService;

    public GetAllLabRequestsQueryHandler(
        IMapper mapper,
        ILabRequestsService labRequestsService,
        ILabParametersService labParametersService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _labParametersService = labParametersService ?? throw new ArgumentNullException(nameof(labParametersService));
        _labRequestsService = labRequestsService ?? throw new ArgumentNullException(nameof(labRequestsService));
    }

    public async Task<GetAllLabRequestsQueryResponse> Handle(
        GetAllLabRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var response = new GetAllLabRequestsQueryResponse();

        List<LabRequest> labRequests = [];

        if (string.IsNullOrWhiteSpace(request.SearchKey))
            labRequests = await _labRequestsService.GetAllAsync();
        else
        {
            string[] tokens = request.SearchKey.Trim().Split();
            labRequests = await _labRequestsService.SearchByTokensAsync(tokens);
        }

        var labParameters = await _labParametersService.GetAllAsync();

        foreach (var labRequest in labRequests)
        {
            var labRequestLiteResponse = _mapper.Map<LabRequestLiteResponse>(labRequest);

            foreach (var labParameterId in labRequest.LabParameterIds)
            {
                var labParameter = labParameters.FirstOrDefault(labParameter => labParameter.Id == labParameterId);

                if (labParameter is null) continue;

                var labRequestLabParameterLiteResponse = _mapper.Map<LabRequestLabParameterLiteResponse>(labParameter);

                labRequestLiteResponse.LabParameters.Add(labRequestLabParameterLiteResponse);
            }

            response.LabRequests.Add(labRequestLiteResponse);
        }

        return response;
    }
}