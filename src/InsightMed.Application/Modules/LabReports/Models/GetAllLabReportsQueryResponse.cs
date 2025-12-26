namespace InsightMed.Application.Modules.LabReports.Models;

public sealed class GetAllLabReportsQueryResponse
{
    public List<LabReportLiteResponse> LabReports { get; set; } = [];
}
