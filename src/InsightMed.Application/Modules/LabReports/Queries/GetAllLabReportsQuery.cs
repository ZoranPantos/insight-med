using AutoMapper;
using InsightMed.Application.Modules.LabReports.Models;
using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using InsightMed.Domain.Entities;
using MediatR;

namespace InsightMed.Application.Modules.LabReports.Queries;

public sealed record GetAllLabReportsQuery(string? SearchKey) : IRequest<GetAllLabReportsQueryResponse>;

public sealed class GetAllLabReportsQueryHandler : IRequestHandler<GetAllLabReportsQuery, GetAllLabReportsQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly ILabReportsService _labReportsService;

    public GetAllLabReportsQueryHandler(IMapper mapper, ILabReportsService labReportsService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _labReportsService = labReportsService ?? throw new ArgumentNullException(nameof(labReportsService));
    }

    public async Task<GetAllLabReportsQueryResponse> Handle(
        GetAllLabReportsQuery request,
        CancellationToken cancellationToken)
    {
        List<LabReport> labReports = [];

        if (string.IsNullOrWhiteSpace(request.SearchKey))
            labReports = await _labReportsService.GetAllAsync();
        else
        {
            string[] tokens = request.SearchKey.Trim().Split();
            labReports = await _labReportsService.SearchByTokensAsync(tokens);
        }

        var response = new GetAllLabReportsQueryResponse
        {
            LabReports = _mapper.Map<List<LabReportLiteResponse>>(labReports)
        };

        return response;
    }
}