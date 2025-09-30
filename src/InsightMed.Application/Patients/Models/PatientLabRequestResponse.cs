using InsightMed.Domain.Enums;

namespace InsightMed.Application.Patients.Models;

public class PatientLabRequestResponse
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public LabRequestState LabRequestState { get; set; }
    public int PatientId { get; set; }
}
