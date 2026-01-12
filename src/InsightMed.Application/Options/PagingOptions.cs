namespace InsightMed.Application.Options;

public sealed class PagingOptions
{
    public int ReportsPageSize { get; set; } = 10;
    public int RequestsPageSize { get; set; } = 10;
    public int PatientsPageSize { get; set; } = 10;
}