namespace InsightMed.Application.Modules.LabReports.Models;

public sealed class ReferenceRangeDto
{
    public double? MinThreshold { get; set; }
    public double? MaxThreshold { get; set; }
    public bool? Positive { get; set; }
}