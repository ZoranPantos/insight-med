using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.Patients.Services.Abstractions;

public interface IPatientsService
{
    Task<(List<Patient> Items, int TotalCount)> GetAllAsync();
    Task<(List<Patient> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize);
    Task<Patient?> GetByIdAsync(int id);
    Task AddAsync(Patient patient);
    Task<(List<Patient> Items, int TotalCount)> SearchByTokensPagedAsync(string[] tokens, int pageNumber, int pageSize);
}