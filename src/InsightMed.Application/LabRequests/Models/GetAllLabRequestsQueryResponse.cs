namespace InsightMed.Application.LabRequests.Models;

public sealed class GetAllLabRequestsQueryResponse
{
    public List<LabRequestLiteResponse> LabRequests { get; set; } = [];
}
