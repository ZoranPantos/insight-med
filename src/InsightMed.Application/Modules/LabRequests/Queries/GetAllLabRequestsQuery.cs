using AutoMapper;
using InsightMed.Application.Modules.LabParameters.Services.Abstractions;
using InsightMed.Application.Modules.LabRequests.Models;
using InsightMed.Application.Modules.LabRequests.Services.Abstractions;
using InsightMed.Domain.Enums;
using MediatR;

namespace InsightMed.Application.Modules.LabRequests.Queries;

public sealed record GetAllLabRequestsQuery : IRequest<GetAllLabRequestsQueryResponse>;

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

        var labRequests = await _labRequestsService.GetAllAsync();
        var labParameters = await _labParametersService.GetAllAsync();

        foreach (var labRequest in labRequests)
        {
            var labRequestLiteResponse = _mapper.Map<LabRequestLiteResponse>(labRequest);

            if (labRequest.LabRequestState == LabRequestState.Completed && labRequest.LabReport is not null)
                labRequestLiteResponse.LabReportId = labRequest.LabReport.Id;

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