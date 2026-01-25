namespace InsightMed.Infrastructure.Options;

public sealed class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public string QueueName { get; set; } = "rpc_queue";
    public int Port { get; set; } = 5672;

    public PublishingOptions Publishing { get; set; } = new();
}

public sealed class PublishingOptions
{
    public bool Mandatory { get; set; }
}