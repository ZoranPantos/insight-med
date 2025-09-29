using InsightMed.Domain.Enums;

namespace InsightMed.Domain.Entities;

public class LabRequest
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public LabRequestState LabRequestState { get; set; }
    public LabReport? LabReport { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    // TODO: Add requester property
}
