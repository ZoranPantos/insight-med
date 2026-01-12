namespace InsightMed.Application.Common.Models;

public abstract class BasePagedResponse
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}