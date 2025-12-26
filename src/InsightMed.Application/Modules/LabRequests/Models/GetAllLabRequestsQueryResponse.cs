namespace InsightMed.Application.Modules.LabRequests.Models;

public sealed class GetAllLabRequestsQueryResponse
{
    public List<LabRequestLiteResponse> LabRequests { get; set; } = [];
}
