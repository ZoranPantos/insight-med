using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using InsightMed.API;
using InsightMed.Application;
using InsightMed.Infrastructure;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApi();
builder.Services.ConfigureApplication();
builder.Services.ConfigureInfrastructure();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "InsightMed")
    .WriteTo.Console()
    .WriteTo.Elasticsearch(
        [new Uri("http://localhost:9200")],
        opts =>
        {
            opts.DataStream = new DataStreamName("logs", "insightmed", "development");
        }
    )
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Open Api V1");
    });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();