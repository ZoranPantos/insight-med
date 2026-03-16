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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog();

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<ILabDbService, LabDbService>();
builder.Services.AddTransient<IParameterValueRandomizerService, ParameterValueRandomizerService>();

builder.Services.AddHostedService<RpcServerWorker>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/lab-parameters", async (ILabDbService labDbService, CancellationToken ct) =>
{
    var parameters = await labDbService.GetAllAsync(ct);

    return Results.Ok(parameters.Select(p => new
    {
        p.Id,
        p.Name,
        reference = new
        {
            p.Reference.MinThreshold,
            p.Reference.MaxThreshold,
            p.Reference.Positive,
            p.Reference.Unit
        }
    }));
});

await app.RunAsync();