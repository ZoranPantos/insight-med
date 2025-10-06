using AutoMapper;
using InsightMed.Application.LabReports.Models;
using InsightMed.Application.LabReports.Services.Abstactions;
using MediatR;

namespace InsightMed.Application.LabReports.Queries;

public sealed record class GetAllLabReportsByPatientIdQuery(int PatientId) : IRequest<GetAllLabReportsQueryResponse>;

public sealed class GetAllLabReportsByPatientIdQueryHandler
    : IRequestHandler<GetAllLabReportsByPatientIdQuery, GetAllLabReportsQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly ILabReportsService _labReportsService;

    public GetAllLabReportsByPatientIdQueryHandler(IMapper mapper, ILabReportsService labReportsService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _labReportsService = labReportsService ?? throw new ArgumentNullException(nameof(labReportsService));
    }

    public async Task<GetAllLabReportsQueryResponse> Handle(
        GetAllLabReportsByPatientIdQuery request,
        CancellationToken cancellationToken)
    {
        var labReports = await _labReportsService.GetAllByPatientIdAsync(request.PatientId);

        var response = new GetAllLabReportsQueryResponse
        {
            LabReports = _mapper.Map<List<LabReportLiteResponse>>(labReports)
        };

        return response;
    }
}