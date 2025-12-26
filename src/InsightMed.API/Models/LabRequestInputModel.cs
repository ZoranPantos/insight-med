namespace InsightMed.API.DTOs;

public sealed class LabRequestInputModel
{
    public int PatientId { get; set; }
    public List<int> LabParameterIds { get; set; } = [];
}
