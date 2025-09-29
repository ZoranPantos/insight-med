using InsightMed.Application.AppManagement.Commands;
using InsightMed.Application.AppManagement.Services.Abstractions;
using InsightMed.Infrastructure.AppManagement.Services;
using InsightMed.Integration.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>();

// TODO: Split configuration into extension methods per project (see Betbuilder for reference)

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.RegisterServicesFromAssembly(typeof(SeedDatabaseCommand).Assembly);
});

builder.Services.AddScoped<IDatabaseManagementService, DatabaseManagementService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Open Api V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
