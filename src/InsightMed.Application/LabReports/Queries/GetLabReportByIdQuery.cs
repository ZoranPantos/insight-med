using AutoMapper;
using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.LabReports.Models;
using InsightMed.Application.LabReports.Services.Abstactions;
using MediatR;

namespace InsightMed.Application.LabReports.Queries;

public sealed record GetLabReportByIdQuery(int Id) : IRequest<GetLabReportByIdQueryResponse>;

public sealed class GetLabReportByIdQueryHandler : IRequestHandler<GetLabReportByIdQuery, GetLabReportByIdQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly ILabReportsService _labReportsService;

    public GetLabReportByIdQueryHandler(IMapper mapper, ILabReportsService labReportsService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _labReportsService = labReportsService ?? throw new ArgumentNullException(nameof(labReportsService));
    }

    public async Task<GetLabReportByIdQueryResponse> Handle(GetLabReportByIdQuery request, CancellationToken cancellationToken)
    {
        var labReport = await _labReportsService.GetByIdAsync(request.Id) ??
            throw new ResourceNotFoundException($"Lab report with ID {request.Id} not found");

        var response = _mapper.Map<GetLabReportByIdQueryResponse>(labReport);

        return response;
    }
}