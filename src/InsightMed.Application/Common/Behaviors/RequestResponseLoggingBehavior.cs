using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace InsightMed.Application.Common.Behaviors;

public sealed class RequestResponseLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly ILogger<RequestResponseLoggingBehavior<TRequest, TResponse>> _logger;

    public RequestResponseLoggingBehavior(ILogger<RequestResponseLoggingBehavior<TRequest, TResponse>> logger) =>
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        var correlationId = Guid.NewGuid();
        string requestName = typeof(TRequest).Name;
        string requestJson = JsonSerializer.Serialize(request);

        _logger.LogInformation(
            "Handling request {RequestName} {CorrelationId}: {@RequestJson}",
            requestName, correlationId, request);

        var stopwatch = Stopwatch.StartNew();
        var response = await next(cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();

        _logger.LogInformation(
            "Response for {RequestName} {CorrelationId}: {@ResponseJson} in {ElapsedMilliseconds} ms",
            requestName, correlationId, response, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
