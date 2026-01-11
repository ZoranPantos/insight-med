using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;

namespace InsightMed.Application.Modules.LabRequests.Services.Abstractions;

public interface ILabRequestsService
{
    Task<List<LabRequest>> GetAllAsync();
    Task AddAsync(LabRequest labRequest);
    Task<int?> SetStateAsync(int id, LabRequestState state);
    Task<List<LabRequest>> SearchByTokensAsync(string[] tokens);
}