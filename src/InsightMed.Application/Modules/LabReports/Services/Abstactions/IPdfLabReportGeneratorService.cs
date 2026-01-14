using InsightMed.Application.Modules.LabReports.Models;
using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.LabReports.Services.Abstactions;

public interface IPdfLabReportGeneratorService
{
    byte[] GenerateLabReportPdf(LabReport report, List<ReportItemDto> parsedContent);
}