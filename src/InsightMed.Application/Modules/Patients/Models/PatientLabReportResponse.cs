namespace InsightMed.Application.Modules.Patients.Models;

public sealed class PatientLabReportResponse
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public int? LabRequestId { get; set; }
    public int PatientId { get; set; }
}
