namespace InsightMed.Domain.Entities;

public class LabReport
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public int? LabRequestId { get; set; }
    public LabRequest? LabRequest { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; }
}
