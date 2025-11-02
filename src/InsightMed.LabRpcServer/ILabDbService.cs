namespace InsightMed.LabRpcServer;

internal interface ILabDbService
{
    Task EnsureInitializedAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LabParameter>> GetAllAsync(CancellationToken cancellationToken = default);
}
