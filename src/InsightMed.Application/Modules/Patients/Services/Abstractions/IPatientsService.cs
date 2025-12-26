using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.Patients.Services.Abstractions;

public interface IPatientsService
{
    Task<List<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(int id);
}
