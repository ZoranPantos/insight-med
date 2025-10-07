namespace InsightMed.API.DTOs;

public sealed class LabRequestInputDTO
{
    public int PatientId { get; set; }
    public List<int> LabParameterIds { get; set; } = [];
}
