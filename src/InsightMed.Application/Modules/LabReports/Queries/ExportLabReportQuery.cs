using InsightMed.Application.Common.Exceptions;
using InsightMed.Application.Modules.LabReports.Models;
using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using MediatR;
using System.Text.Json;

namespace InsightMed.Application.Modules.LabReports.Queries;

public sealed record ExportLabReportQuery(int Id) : IRequest<ExportLabReportQueryResponse>;

public sealed class ExportLabReportQueryHandler : IRequestHandler<ExportLabReportQuery, ExportLabReportQueryResponse>
{
    private readonly ILabReportsService _labReportsService;
    private readonly IPdfLabReportGeneratorService _pdfGenerator;

    public ExportLabReportQueryHandler(ILabReportsService labReportsService, IPdfLabReportGeneratorService pdfGenerator)
    {
        _labReportsService = labReportsService ?? throw new ArgumentNullException(nameof(labReportsService));
        _pdfGenerator = pdfGenerator ?? throw new ArgumentNullException(nameof(pdfGenerator));
    }

    public async Task<ExportLabReportQueryResponse> Handle(ExportLabReportQuery request, CancellationToken cancellationToken)
    {
        var labReport = await _labReportsService.GetByIdAsync(request.Id) ??
            throw new ResourceNotFoundException($"Lab report with ID {request.Id} not found");

        var parsedContent = JsonSerializer.Deserialize<List<ReportItemDto>>(labReport.Content) ??
            throw new Exception($"Deserialized Content of the lab report with ID {request.Id} was null");

        var pdfBytes = _pdfGenerator.GenerateLabReportPdf(labReport, parsedContent);

        return new ExportLabReportQueryResponse
        {
            FileName = $"Report_{labReport.Patient.Uid}_{labReport.Created:yyyyMMdd}.pdf",
            ContentType = "application/pdf",
            Data = pdfBytes
        };
    }
}