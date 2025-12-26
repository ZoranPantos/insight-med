namespace InsightMed.Application.Modules.Patients.Models;

public sealed class GetAllPatientsQueryResponse
{
    public List<PatientLiteResponse> Patients { get; set; } = [];
}
