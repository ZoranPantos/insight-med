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
        List<LabReport> labReports = [];

        if (string.IsNullOrWhiteSpace(request.SearchKey))
            labReports = await _labReportsService.GetAllPagedAsync(pageNumber, pageSize);
        else
        {
            string[] tokens = request.SearchKey.Trim().Split();
            labReports = await _labReportsService.SearchByTokensPagedAsync(tokens, pageNumber, pageSize);
        }

        var response = new GetAllLabReportsQueryResponse
        {
            LabReports = _mapper.Map<List<LabReportLiteResponse>>(labReports),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = await _labReportsService.GetTotalLabReportCountAsync()
        };

        return response;
    }
}