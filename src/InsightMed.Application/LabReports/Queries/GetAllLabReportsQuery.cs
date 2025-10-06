using AutoMapper;
using InsightMed.Application.LabReports.Models;
using InsightMed.Application.LabReports.Services.Abstactions;
using MediatR;

namespace InsightMed.Application.LabReports.Queries;

public sealed record GetAllLabReportsQuery : IRequest<GetAllLabReportsQueryResponse>;

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
        var labReports = await _labReportsService.GetAllAsync();

        var response = new GetAllLabReportsQueryResponse
        {
            LabReports = _mapper.Map<List<LabReportLiteResponse>>(labReports)
        };

        return response;
    }
}

