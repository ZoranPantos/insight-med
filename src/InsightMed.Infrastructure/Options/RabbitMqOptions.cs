namespace InsightMed.Infrastructure.Options;

public sealed class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public string QueueName { get; set; } = "rpc_queue";
}
