using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.Patients.Services.Abstractions;

public interface IPatientsService
{
    Task<(List<Patient> Items, int TotalCount)> GetAllAsync();
    Task<(List<Patient> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize);
    Task<(Patient? Patient, List<LabRequest> PagedLabRequests, int TotalCount)> GetByIdWithLabRequestsPagedAsync(int id, int pageNumber, int pageSize);
    Task AddAsync(Patient patient);
    Task<(List<Patient> Items, int TotalCount)> SearchByTokensPagedAsync(string[] tokens, int pageNumber, int pageSize);
}