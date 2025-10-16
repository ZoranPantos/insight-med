using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace InsightMed.LabRpcServer;

// TODO: Configure logs to go to elastic search with Serilog

// TODO: Implement actual business logic instead of test response

// TODO: Connect to database

internal sealed class RpcServerWorker : BackgroundService
{
    private readonly ILogger<RpcServerWorker> _logger;
    private readonly RabbitMqOptions _options;

    private IConnectionFactory? _factory;
    private IConnection? _connection;
    private IChannel? _channel;

    public RpcServerWorker(ILogger<RpcServerWorker> logger, IOptions<RabbitMqOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _factory = new ConnectionFactory { HostName = _options.HostName };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await StartConsumingAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RPC server error. Retrying in 3s ...");
                await Task.Delay(3000, stoppingToken);
            }
            finally
            {
                await SafeCloseAsync();
            }
        }
    }

    private async Task SafeCloseAsync()
    {
        if (_channel is not null) await _channel.CloseAsync();
        if (_connection is not null) await _connection.CloseAsync();

        _channel = null;
        _connection = null;
    }

    private async Task StartConsumingAsync(CancellationToken stoppingToken)
    {
        _connection = await _factory!.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: _options.QueueName, durable: false, exclusive: false,
            autoDelete: false, arguments: null);

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnReceivedAsync;

        await _channel.BasicConsumeAsync(_options.QueueName, false, consumer);

        _logger.LogInformation("RPC Server started. Listening on {Queue}", _options.QueueName);

        while (!stoppingToken.IsCancellationRequested && _connection.IsOpen)
            await Task.Delay(500, stoppingToken);
    }

    private async Task OnReceivedAsync(object? sender, BasicDeliverEventArgs ea)
    {
        if (sender is not AsyncEventingBasicConsumer cons || cons.Channel is null)
        {
            _logger.LogWarning("Invalid consumer or channel. Skipping message");
            return;
        }

        var ch = cons.Channel;
        string response = string.Empty;

        byte[] body = ea.Body.ToArray();
        var props = ea.BasicProperties;
        var replyProps = new BasicProperties { CorrelationId = props.CorrelationId };

        try
        {
            // TODO: here do business logic
            string message = Encoding.UTF8.GetString(body);
            response = $"Response for message {message}";
        }
        catch (Exception ex)
        {
            _logger.LogError("An exception occurred while processing the message: {ExceptionMessage}", ex.Message);
        }
        finally
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);

            await ch.BasicPublishAsync(
                exchange: string.Empty, routingKey: props.ReplyTo!,
                mandatory: true, basicProperties: replyProps, body: responseBytes);

            await ch.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }
}
