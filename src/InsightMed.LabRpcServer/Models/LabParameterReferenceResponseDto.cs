namespace InsightMed.LabRpcServer.Models;

internal sealed class LabParameterReferenceResponseDto
{
    public double? MinThreshold { get; set; }
    public double? MaxThreshold { get; set; }
    public bool? Positive { get; set; }
}