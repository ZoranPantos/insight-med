namespace InsightMed.Application.Patients.Models;

public sealed class GetAllPatientsQueryResponse
{
    public List<PatientLiteResponse> Patients { get; set; } = [];
}
