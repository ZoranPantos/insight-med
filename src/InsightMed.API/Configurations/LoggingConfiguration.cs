using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Serilog;
using Serilog.Events;

namespace InsightMed.API.Configurations;

public static class LoggingConfiguration
{
    public static void ConfigureAndCreateLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "InsightMed.API")
            .WriteTo.Console()
            .WriteTo.Elasticsearch(
                [new Uri("http://localhost:9200")],
                opts =>
                {
                    opts.DataStream = new DataStreamName(
                        "logs",
                        "insightmed",
                        "development");
                })
            .CreateLogger();
    }
}
