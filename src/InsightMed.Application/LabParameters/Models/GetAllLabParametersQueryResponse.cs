namespace InsightMed.Application.LabParameters.Models;

public sealed class GetAllLabParametersQueryResponse
{
    public List<LabParameterResponse> LabParameters { get; set; } = [];
}
