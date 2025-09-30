using InsightMed.Domain.Enums;

namespace InsightMed.Domain.Entities;

// TODO: Add more info - contact for example
public class Patient
{
    public int Id { get; set; }
    public string Uid { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public BloodGroup BloodGroup { get; set; }

    public List<LabReport> LabReports { get; set; } = [];
    public List<LabRequest> LabRequests { get; set; } = [];
}
