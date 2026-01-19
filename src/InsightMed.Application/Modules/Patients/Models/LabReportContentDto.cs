namespace InsightMed.Application.Modules.Patients.Models;

public sealed class LabReportContentDto : List<ContentItem>
{
}

public class ContentItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool? IsPositive { get; set; }
    public double? Measurement { get; set; }
    public ReferenceRange Reference { get; set; } = new();
}

public class ReferenceRange
{
    public double? MinThreshold { get; set; }
    public double? MaxThreshold { get; set; }
    public bool? Positive { get; set; }
    public string? Unit { get; set; }
}