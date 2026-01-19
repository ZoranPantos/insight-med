namespace InsightMed.Application.Modules.Patients.Models;

public sealed class LabParameterReferenceResponse
{
    public double? MinThreshold { get; set; }
    public double? MaxThreshold { get; set; }
    public bool? Positive { get; set; }
}