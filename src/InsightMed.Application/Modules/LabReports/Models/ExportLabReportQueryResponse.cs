namespace InsightMed.Application.Modules.LabReports.Models;

public sealed class ExportLabReportQueryResponse
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Data { get; set; } = [];
}