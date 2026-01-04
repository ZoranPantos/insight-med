namespace InsightMed.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Seen { get; set; }
    public string RequesterId { get; set; } = string.Empty;

    public int LabReportId { get; set; }
    public LabReport LabReport { get; set; }
}