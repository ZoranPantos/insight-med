namespace InsightMed.Application.Common.Abstractions.Messaging;

public interface ILabRpcClient
{
    Task<string> CallAsync(string message, CancellationToken cancellationToken = default);
}
