namespace InsightMed.Application.Modules.Notifications.Models;

public sealed class NotificationLiteResponse
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public int LabReportId { get; set; }
}