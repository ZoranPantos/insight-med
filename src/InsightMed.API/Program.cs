using InsightMed.API.Configurations;
using InsightMed.API.Extensions;
using InsightMed.Application;
using InsightMed.Infrastructure;
using Serilog;

try
{
    Console.Title = "InsightMed.API";
}
catch { }

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApi();
builder.Services.ConfigureApplication(builder.Configuration);
builder.Services.ConfigureInfrastructure(builder.Configuration);

LoggingConfiguration.ConfigureAndCreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

await app.ApplyMigrationsAsync();

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