using InsightMed.Application.Common.Models;

namespace InsightMed.Application.Modules.LabParameters.Models;

public sealed class GetAllLabParametersQueryResponse : BaseCachedResponse
{
    public List<LabParameterResponse> LabParameters { get; set; } = [];
}