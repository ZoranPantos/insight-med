namespace InsightMed.LabRpcServer.Options;

internal sealed class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public string QueueName { get; set; } = "rpc_queue";
    public bool AutoAck { get; set; }

    public AdditionalQueueOptions AdditionalQueueOptions { get; set; } = new();
    public QosOptions Qos { get; set; } = new();
}

internal sealed class AdditionalQueueOptions
{
    public bool Durable { get; set; }
    public bool Exclusive { get; set; }
    public bool AutoDelete { get; set; }
}

internal sealed class QosOptions
{
    public uint PrefetchSize { get; set; }
    public ushort PrefetchCount { get; set; }
    public bool Global { get; set; }
}