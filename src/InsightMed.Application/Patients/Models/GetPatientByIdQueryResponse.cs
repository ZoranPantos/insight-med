using InsightMed.Domain.Enums;

namespace InsightMed.Application.Patients.Models;

public class GetPatientByIdQueryResponse
{
    public int Id { get; set; }
    public string Uid { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public BloodGroup BloodGroup { get; set; }

    public List<PatientLabReportResponse> LabReports { get; set; } = [];
    public List<PatientLabRequestResponse> LabRequests { get; set; } = [];
}
