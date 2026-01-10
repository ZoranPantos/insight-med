using InsightMed.LabRpcServer.Models;
using InsightMed.LabRpcServer.Options;
using InsightMed.LabRpcServer.Services.Abstractions;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace InsightMed.LabRpcServer;

internal sealed class RpcServerWorker : BackgroundService
{
    private readonly ILogger<RpcServerWorker> _logger;
    private readonly RabbitMqOptions _options;
    private readonly ILabDbService _labDbService;
    private readonly IParameterValueRandomizerService _randomizerService;
    private readonly IConfiguration _configuration;

    private IConnectionFactory? _factory;
    private IConnection? _connection;
    private IChannel? _channel;

    public RpcServerWorker(
        ILogger<RpcServerWorker> logger,
        IOptions<RabbitMqOptions> options,
        ILabDbService labDbService,
        IParameterValueRandomizerService randomizerService,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _labDbService = labDbService ?? throw new ArgumentNullException(nameof(labDbService));
        _randomizerService = randomizerService ?? throw new ArgumentNullException(nameof(randomizerService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    protected async override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RPC Server started");

        await _labDbService.EnsureInitializedAsync(cancellationToken);

        _factory = new ConnectionFactory { HostName = _options.HostName };

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await StartConsumingAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RPC server error. Retrying in 3s ...");
                await Task.Delay(3000, cancellationToken);
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

    private async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        _connection = await _factory!.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: _options.AdditionalQueueOptions.Durable,
            exclusive: _options.AdditionalQueueOptions.Exclusive,
            autoDelete: _options.AdditionalQueueOptions.AutoDelete,
            arguments: null,
            cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(
            prefetchSize: _options.Qos.PrefetchSize,
            prefetchCount: _options.Qos.PrefetchCount,
            global: _options.Qos.Global,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnReceivedAsync;

        await _channel.BasicConsumeAsync(
            _options.QueueName,
            _options.AutoAck,
            consumer,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Message consumption started. Listening on {Queue}", _options.QueueName);

        while (!cancellationToken.IsCancellationRequested && _connection.IsOpen)
            await Task.Delay(500, cancellationToken);
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
            string labRequestJson = Encoding.UTF8.GetString(body);

            var labRequest = JsonSerializer.Deserialize<LabRequestDto>(labRequestJson) ??
                throw new InvalidOperationException("Deserialization of lab request failed");

            var labParameters = await _labDbService.GetByIdsAsync(labRequest.LabParameterIds);

            var labResponse = new LabResponseDto();

            foreach (var labParameter in labParameters)
            {
                var randomizerResult = _randomizerService.Randomize(labParameter.Reference);

                var labParameterValueResponseDto = new LabParameterValueResponseDto
                {
                    Id = labParameter.Id,
                    Name = labParameter.Name,
                    IsPositive = randomizerResult.IsPositive,
                    Measurement = randomizerResult.Value,
                    Reference = new()
                    {
                        MinThreshold = labParameter.Reference.MinThreshold,
                        MaxThreshold = labParameter.Reference.MaxThreshold,
                        Positive = labParameter.Reference.Positive
                    }
                };

                labResponse.LabParameterValueResponseDtos.Add(labParameterValueResponseDto);
            }

            response = JsonSerializer.Serialize(labResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError("An exception occurred while processing the message: {ExceptionMessage}", ex.Message);
        }
        finally
        {
            int randomDelayMs = GetRandomDelayMilliseconds();
            await Task.Delay(randomDelayMs);

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);

            await ch.BasicPublishAsync(
                exchange: string.Empty, routingKey: props.ReplyTo!,
                mandatory: true, basicProperties: replyProps, body: responseBytes);

            await ch.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        }
    }

    private int GetRandomDelayMilliseconds()
    {
        int defaultMin = 5;
        int defaultMax = 10;

        int min = _configuration
            .GetSection("LabResultsDelaySimulationParameters")
            .GetValue<int?>("MinSeconds") ?? defaultMin;

        int max = _configuration
            .GetSection("LabResultsDelaySimulationParameters")
            .GetValue<int?>("MaxSeconds") ?? defaultMax;

        int randomDelayMs = new Random().Next(min, max) * 1000;

        return randomDelayMs;
    }
}