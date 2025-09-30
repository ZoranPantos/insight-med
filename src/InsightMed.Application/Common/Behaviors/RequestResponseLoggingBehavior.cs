using MediatR;
using System.Diagnostics;
using System.Text.Json;

namespace InsightMed.Application.Common.Behaviors;

public sealed class RequestResponseLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    // TODO: Inject and use logger instead of console logging

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        var correlationId = Guid.NewGuid();
        string requestName = typeof(TRequest).Name;
        string requestJson = JsonSerializer.Serialize(request);

        Console.WriteLine($"Handling request {requestName} {correlationId}: {requestJson}");

        var stopwatch = Stopwatch.StartNew();
        var response = await next(cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();

        string responseJson = JsonSerializer.Serialize(response);

        Console.WriteLine($"Response for {requestName} {correlationId}: {responseJson} in {stopwatch.ElapsedMilliseconds}ms");

        return response;
    }
}
