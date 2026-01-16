namespace InsightMed.Application.Modules.Patients.Models;

public sealed class GetEvaluatedParametersByPatientIdQueryResponse
{
    public List<EvaluatedLabParameterResponse> EvaluatedLabParameters { get; set; } = [];
}