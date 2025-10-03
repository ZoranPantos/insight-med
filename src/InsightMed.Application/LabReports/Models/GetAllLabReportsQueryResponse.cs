namespace InsightMed.Application.LabReports.Models;

public sealed class GetAllLabReportsQueryResponse
{
    public List<LabReportLiteResponse> LabReports { get; set; } = [];
}
