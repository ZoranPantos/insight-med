namespace InsightMed.Application.Modules.LabReports.Models;

internal sealed class LabReportRpcServerResponse
{
    public List<LabParameterValueRpcServerResponse> LabParameterValues { get; set; } = [];
}