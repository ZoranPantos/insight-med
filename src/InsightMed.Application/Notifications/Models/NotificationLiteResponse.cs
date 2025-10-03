namespace InsightMed.Application.Notifications.Models;

public sealed class NotificationLiteResponse
{
    public string Message { get; set; } = string.Empty;
    public int LabReportId { get; set; }
}
