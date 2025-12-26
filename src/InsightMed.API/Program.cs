using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using InsightMed.API;
using InsightMed.Application;
using InsightMed.Infrastructure;
using InsightMed.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApi();
builder.Services.ConfigureApplication(builder.Configuration);
builder.Services.ConfigureInfrastructure(builder.Configuration);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "InsightMed")
    .WriteTo.Console()
    .WriteTo.Elasticsearch(
        [new Uri("http://localhost:9200")],
        opts => { opts.DataStream = new DataStreamName("logs", "insightmed", "development"); }
    )
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

await ApplyMigrationsAsync(app);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "Open Api V1"); });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

static async Task ApplyMigrationsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Database migration step completed.");
    }
    catch (SqlException ex) when (ex.Number == 1801)
    {
        logger.LogWarning(ex, "Database {Database} already exists; skipping creation.", dbContext.Database.GetDbConnection().Database);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Errors in database migration step.");
        throw;
    }
}