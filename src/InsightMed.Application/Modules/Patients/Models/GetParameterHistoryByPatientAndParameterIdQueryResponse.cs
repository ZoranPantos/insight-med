namespace InsightMed.Application.Modules.Patients.Models;

public sealed class GetParameterHistoryByPatientAndParameterIdQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public LabParameterReferenceResponse LabParameterReference { get; set; } = new();
    public List<LabParameterHistoryRecordResponse> History { get; set; } = [];
}