using InsightMed.LabRpcServer.Models;

namespace InsightMed.LabRpcServer.Services.Abstractions;

internal interface ILabDbService
{
    Task EnsureInitializedAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LabParameter>> GetAllAsync(CancellationToken cancellationToken = default);
}
