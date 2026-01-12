using AutoMapper;
using InsightMed.Application.Modules.LabReports.Models;
using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using InsightMed.Application.Options;
using InsightMed.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace InsightMed.Application.Modules.LabReports.Queries;

public sealed record GetAllLabReportsQuery(string? SearchKey, int PageNumber) : IRequest<GetAllLabReportsQueryResponse>;

public sealed class GetAllLabReportsQueryHandler : IRequestHandler<GetAllLabReportsQuery, GetAllLabReportsQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly ILabReportsService _labReportsService;
    private readonly PagingOptions _pagingOptions;

    public GetAllLabReportsQueryHandler(
        IMapper mapper,
        ILabReportsService labReportsService,
        IOptions<PagingOptions> pagingOptions)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _labReportsService = labReportsService ?? throw new ArgumentNullException(nameof(labReportsService));
        _pagingOptions = pagingOptions.Value ?? throw new ArgumentNullException(nameof(pagingOptions));
    }

    public async Task<GetAllLabReportsQueryResponse> Handle(
        GetAllLabReportsQuery request,
        CancellationToken cancellationToken)
    {
        int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        int pageSize = _pagingOptions.ReportsPageSize;
        int totalCount;
        List<LabReport> labReports = [];

        if (string.IsNullOrWhiteSpace(request.SearchKey))
        {
            var (Items, TotalCount) = await _labReportsService.GetAllPagedAsync(pageNumber, pageSize);
            labReports = Items;
            totalCount = TotalCount;
        }
        else
        {
            string[] tokens = request.SearchKey.Trim().Split();
            var (Items, TotalCount) = await _labReportsService.SearchByTokensPagedAsync(tokens, pageNumber, pageSize);
            labReports = Items;
            totalCount = TotalCount;
        }

        var response = new GetAllLabReportsQueryResponse
        {
            LabReports = _mapper.Map<List<LabReportLiteResponse>>(labReports),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return response;
    }
}