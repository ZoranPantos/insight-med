namespace InsightMed.LabRpcServer;

internal sealed class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public string QueueName { get; set; } = "rpc_queue";
}
