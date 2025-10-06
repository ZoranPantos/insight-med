namespace InsightMed.Application.LabReports.Models;

public sealed class GetLabReportByIdQueryResponse
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Created {  get; set; }
    public string PatientFullName { get; set; } = string.Empty;
    public string PatientUid { get; set; } = string.Empty;
}
