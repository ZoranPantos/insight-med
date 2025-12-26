using InsightMed.Application.Common.Models;

namespace InsightMed.Application.LabParameters.Models;

public sealed class GetAllLabParametersQueryResponse : BaseResponse
{
    public List<LabParameterResponse> LabParameters { get; set; } = [];
}
