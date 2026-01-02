using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using InsightMed.LabRpcServer;
using InsightMed.LabRpcServer.Options;
using InsightMed.LabRpcServer.Services;
using InsightMed.LabRpcServer.Services.Abstractions;
using Serilog;
using Serilog.Events;

Console.Title = "InsightMed.LabRpcServer";

Log.Logger = new LoggerConfiguration()
  .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
  .MinimumLevel.Information()
  .Enrich.FromLogContext()
  .Enrich.WithProperty("Application", "InsightMed.LabRpcServer")
  .WriteTo.Console()
  .WriteTo.Elasticsearch(
    [new Uri("http://localhost:9200")],
    opts =>
    {
        opts.DataStream = new DataStreamName(
          "logs",
          "insightmed",
          "development"
        );
    }
  )
  .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog();

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<ILabDbService, LabDbService>();
builder.Services.AddTransient<IParameterValueRandomizerService, ParameterValueRandomizerService>();

builder.Services.AddHostedService<RpcServerWorker>();

var host = builder.Build();
await host.RunAsync();