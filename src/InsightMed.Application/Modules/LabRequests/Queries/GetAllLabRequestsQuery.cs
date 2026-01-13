using AutoMapper;
using InsightMed.Application.Modules.LabParameters.Services.Abstractions;
using InsightMed.Application.Modules.LabRequests.Models;
using InsightMed.Application.Modules.LabRequests.Services.Abstractions;
using InsightMed.Application.Options;
using InsightMed.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace InsightMed.Application.Modules.LabRequests.Queries;

public sealed record GetAllLabRequestsQuery(string? SearchKey, int PageNumber) : IRequest<GetAllLabRequestsQueryResponse>;

public sealed class GetAllLabRequestsQueryHandler
    : IRequestHandler<GetAllLabRequestsQuery, GetAllLabRequestsQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly ILabRequestsService _labRequestsService;
    private readonly ILabParametersService _labParametersService;
    private readonly PagingOptions _pagingOptions;

    public GetAllLabRequestsQueryHandler(
        IMapper mapper,
        ILabRequestsService labRequestsService,
        ILabParametersService labParametersService,
        IOptions<PagingOptions> pagingOptions)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _labParametersService = labParametersService ?? throw new ArgumentNullException(nameof(labParametersService));
        _labRequestsService = labRequestsService ?? throw new ArgumentNullException(nameof(labRequestsService));
        _pagingOptions = pagingOptions.Value ?? throw new ArgumentNullException(nameof(pagingOptions));
    }

    public async Task<GetAllLabRequestsQueryResponse> Handle(
        GetAllLabRequestsQuery request,
        CancellationToken cancellationToken)
    {
        int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        int pageSize = _pagingOptions.RequestsPageSize;
        int totalCount;
        var response = new GetAllLabRequestsQueryResponse();

        List<LabRequest> labRequests = [];

        if (string.IsNullOrWhiteSpace(request.SearchKey))
        {
            var (Items, TotalCount) = await _labRequestsService.GetAllPagedAsync(pageNumber, pageSize);
            labRequests = Items;
            totalCount = TotalCount;
        }
        else
        {
            string[] tokens = request.SearchKey.Trim().Split();
            var (Items, TotalCount) = await _labRequestsService.SearchByTokensPagedAsync(tokens, pageNumber, pageSize);
            labRequests = Items;
            totalCount = TotalCount;
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

        response.PageNumber = pageNumber;
        response.PageSize = pageSize;
        response.TotalCount = totalCount;

        return response;
    }
}