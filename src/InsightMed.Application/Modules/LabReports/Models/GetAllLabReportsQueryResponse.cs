using InsightMed.Application.Common.Models;

namespace InsightMed.Application.Modules.LabReports.Models;

public sealed class GetAllLabReportsQueryResponse : BasePagedResponse
{
    public List<LabReportLiteResponse> LabReports { get; set; } = [];
}