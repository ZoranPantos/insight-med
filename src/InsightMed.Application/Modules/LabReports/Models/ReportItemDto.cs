namespace InsightMed.Application.Modules.LabReports.Models;

public sealed class ReportItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool? IsPositive { get; set; }
    public double? Measurement { get; set; }
    public ReferenceRangeDto Reference { get; set; } = new();
}