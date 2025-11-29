namespace InsightMed.Application.LabReports.Models;

internal sealed class LabParameterValueRpcServerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool? IsPositive { get; set; }
    public double? Measurement { get; set; }
}