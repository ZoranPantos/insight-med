namespace InsightMed.Application.Patients.Models;

public sealed class PatientLiteResponse
{
    public int Id { get; set; }
    public string Uid { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
