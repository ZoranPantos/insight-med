using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.Patients.Services.Abstractions;

public interface IPatientsService
{
    Task<List<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(int id);
    Task AddAsync(Patient patient);
    Task<List<Patient>> SearchByTokensAsync(string[] tokens);
}