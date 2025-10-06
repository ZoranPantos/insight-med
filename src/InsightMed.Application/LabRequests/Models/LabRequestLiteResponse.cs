using InsightMed.Domain.Enums;

namespace InsightMed.Application.LabRequests.Models;

public sealed class LabRequestLiteResponse
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public LabRequestState LabRequestState { get; set; }
    public string PatientFullName { get; set; } = string.Empty;
    public string PatientUid { get; set; } = string.Empty;
    public List<LabRequestLabParameterLiteResponse> LabParameters { get; set; } = [];
}
