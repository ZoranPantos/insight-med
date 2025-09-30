namespace InsightMed.Application.Patients.Models;

public class GetAllPatientsQueryResponse
{
    public List<PatientLiteResponse> Patients { get; set; } = [];
}
