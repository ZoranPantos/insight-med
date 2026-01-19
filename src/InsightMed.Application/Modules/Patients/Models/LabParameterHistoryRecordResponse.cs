namespace InsightMed.Application.Modules.Patients.Models;

public sealed class LabParameterHistoryRecordResponse
{
    public double? Measurement { get; set; }
    public bool? IsPositive { get; set; }
    public DateTime Created { get; set; }
}