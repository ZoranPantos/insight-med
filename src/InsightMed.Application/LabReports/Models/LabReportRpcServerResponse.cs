namespace InsightMed.Application.LabReports.Models;

internal sealed class LabReportRpcServerResponse
{
    public List<LabParameterValueRpcServerResponse> LabParameterValues { get; set; } = [];
}