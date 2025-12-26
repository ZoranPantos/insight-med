namespace InsightMed.Application.Modules.LabReports.Models;

public sealed class LabReportLiteResponse
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string PatientFullName { get; set; } = string.Empty;
    public string PatientUid { get; set; } = string.Empty;
}
