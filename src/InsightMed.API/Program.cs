using InsightMed.API.Configurations;
using InsightMed.API.Extensions;
using InsightMed.API.Hubs;
using InsightMed.Application;
using InsightMed.Infrastructure;
using Serilog;

Console.Title = "InsightMed.API";

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApi();
builder.Services.ConfigureApplication(builder.Configuration);
builder.Services.ConfigureInfrastructure(builder.Configuration);

LoggingConfiguration.ConfigureAndCreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

app.LogListeningPort();

await app.ApplyMigrationsAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "Open Api V1"); });
}

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notifications");

app.Run();