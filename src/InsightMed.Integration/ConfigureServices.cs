using InsightMed.Application.AppManagement.Services.Abstractions;
using InsightMed.Application.Patients.Services.Abstractions;
using InsightMed.Infrastructure.AppManagement.Services;
using InsightMed.Infrastructure.Patients.Services;
using InsightMed.Integration.Data;
using Microsoft.Extensions.DependencyInjection;

namespace InsightMed.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>();

        services.AddScoped<IDatabaseManagementService, DatabaseManagementService>();
        services.AddScoped<IPatientsService, PatientsService>();

        return services;
    }
}
