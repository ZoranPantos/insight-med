using InsightMed.Domain.Entities;
using InsightMed.Domain.Enums;

namespace InsightMed.Application.Modules.LabRequests.Services.Abstractions;

public interface ILabRequestsService
{
    Task<(List<LabRequest> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize);
    Task AddAsync(LabRequest labRequest);
    Task<int?> SetStateAsync(int id, LabRequestState state);
    Task<(List<LabRequest> Items, int TotalCount)> SearchByTokensPagedAsync(string[] tokens, int pageNumber, int pageSize);
}