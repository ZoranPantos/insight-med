using InsightMed.Application.Common.Abstractions.Messaging;
using InsightMed.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Collections.Concurrent;
using System.Text;

namespace InsightMed.Infrastructure.Messaging;

public sealed class RabbitMqRpcClient : ILabRpcClient, IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqRpcClient> _logger;
    private readonly IConnectionFactory _connectionFactory;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>>_callbackMapper = [];

    private IConnection? _connection;
    private IChannel? _channel;
    private string? _replyQueueName;
    private bool _started;

    public RabbitMqRpcClient(IOptions<RabbitMqOptions> rabbitMqOptions, ILogger<RabbitMqRpcClient> logger)
    {
        _options = rabbitMqOptions.Value ?? throw new ArgumentNullException(nameof(rabbitMqOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _connectionFactory = new ConnectionFactory() { HostName = _options.HostName };
    }

    // Start once and keep the reply consumer alive
    public async Task StartAsync()
    {
        if (_started) return;

        const int maxRetries = 3;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                var queueDeclareResult = await _channel.QueueDeclareAsync();
                _replyQueueName = queueDeclareResult.QueueName;

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += (model, ea) =>
                {
                    string? correlationId = ea.BasicProperties.CorrelationId;

                    if (!string.IsNullOrEmpty(correlationId))
                    {
                        if (_callbackMapper.TryRemove(correlationId, out var tcs))
                        {
                            byte[] body = ea.Body.ToArray();
                            string response = Encoding.UTF8.GetString(body);

                            tcs.TrySetResult(response);
                        }
                    }

                    return Task.CompletedTask;
                };

                await _channel.BasicConsumeAsync(_replyQueueName, true, consumer);

                _started = true;

                _logger.LogInformation("RPC Client started");

                return;
            }
            catch (BrokerUnreachableException ex)
            {
                int delaySeconds = 2;

                if (attempt == maxRetries)
                {
                    _logger.LogError(
                        ex,
                        "Failed to start RabbitMQ RPC client after {Attempt}/{MaxRetries} attempts",
                        attempt,
                        maxRetries);

                    throw;
                }

                _logger.LogWarning(
                    ex,
                    "RabbitMQ broker unreachable when starting RPC client (attempt {Attempt}/{MaxRetries}). Retrying in {Delay}s",
                    attempt,
                    maxRetries,
                    delaySeconds);

                await Task.Delay(delaySeconds * 1000);
            }
        }
    }

    public async Task<string> CallAsync(string message, CancellationToken cancellationToken = default)
    {
        // Lazy-start if the hosted service didn't run yet
        if (!_started) await StartAsync();

        if (_channel is null || _replyQueueName is null)
            throw new InvalidOperationException("RPC client not initialized properly.");

        string correlationId = Guid.NewGuid().ToString();

        var props = new BasicProperties
        {
            CorrelationId = correlationId,
            ReplyTo = _replyQueueName
        };

        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

        if (!_callbackMapper.TryAdd(correlationId, tcs))
            throw new InvalidOperationException("Failed to add correlation mapping.");

        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _options.QueueName,
            mandatory: _options.Publishing.Mandatory,
            basicProperties: props,
            body: messageBytes
        );

        using CancellationTokenRegistration ctr = cancellationToken.Register(() =>
        {
            _callbackMapper.TryRemove(correlationId, out _);
            tcs.TrySetCanceled(cancellationToken);
        });

        return await tcs.Task.ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var (_, tcs) in _callbackMapper)
            tcs.TrySetException(new OperationCanceledException("RPC client is disposing. Request canceled"));

        _callbackMapper.Clear();

        if (_channel is not null) await _channel.CloseAsync();
        if (_connection is not null) await _connection.CloseAsync();
    }
}