namespace InsightMed.Application.Modules.LabReports.Models;

internal sealed class LabParameterReferenceRpcServerResponse
{
    public double? MinThreshold { get; set; }
    public double? MaxThreshold { get; set; }
    public bool? Positive { get; set; }
}