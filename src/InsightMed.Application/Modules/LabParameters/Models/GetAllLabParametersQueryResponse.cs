using InsightMed.Application.Common.Models;

namespace InsightMed.Application.Modules.LabParameters.Models;

public sealed class GetAllLabParametersQueryResponse : BaseResponse
{
    public List<LabParameterResponse> LabParameters { get; set; } = [];
}