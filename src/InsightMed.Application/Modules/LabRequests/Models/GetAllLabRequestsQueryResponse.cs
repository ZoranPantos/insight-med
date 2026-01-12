using InsightMed.Application.Common.Models;

namespace InsightMed.Application.Modules.LabRequests.Models;

public sealed class GetAllLabRequestsQueryResponse : BasePagedResponse
{
    public List<LabRequestLiteResponse> LabRequests { get; set; } = [];
}