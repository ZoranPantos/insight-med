using InsightMed.Domain.Entities;

namespace InsightMed.Application.LabRequests.Services.Abstractions;

public interface ILabRequestsService
{
    Task<List<LabRequest>> GetAllAsync();
    Task AddAsync(LabRequest labRequest);
}
