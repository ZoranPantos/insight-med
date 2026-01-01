namespace InsightMed.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Seen { get; set; }

    public int LabReportId { get; set; }
    public LabReport LabReport { get; set; }
}
