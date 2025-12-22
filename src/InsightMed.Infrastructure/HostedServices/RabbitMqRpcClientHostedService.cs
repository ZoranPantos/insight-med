using InsightMed.Infrastructure.Messaging;
using Microsoft.Extensions.Hosting;

namespace InsightMed.Infrastructure.HostedServices;

public sealed class RabbitMqRpcClientHostedService : IHostedService
{
    private readonly RabbitMqRpcClient _client;

    public RabbitMqRpcClientHostedService(RabbitMqRpcClient client) => _client = client;

    public async Task StartAsync(CancellationToken cancellationToken) => await _client.StartAsync();

    public async Task StopAsync(CancellationToken cancellationToken) => await _client.DisposeAsync();
}
