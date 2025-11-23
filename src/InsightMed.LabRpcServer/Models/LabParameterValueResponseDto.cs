namespace InsightMed.LabRpcServer.Models;

internal sealed class LabParameterValueResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool? IsPositive { get; set; }
    public double? Measurement { get; set; }
}
