using InsightMed.Domain.Entities;

namespace InsightMed.Application.Patients.Services.Abstractions;

public interface IPatientsService
{
    Task<List<Patient>> GetAllPatientsAsync();
    Task<Patient?> GetPatientByIdAsync(long id);
}
