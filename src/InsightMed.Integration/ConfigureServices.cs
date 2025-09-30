using InsightMed.Application.AppManagement.Services.Abstractions;
using InsightMed.Infrastructure.AppManagement.Services;
using InsightMed.Integration.Data;
using Microsoft.Extensions.DependencyInjection;

namespace InsightMed.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>();
        services.AddScoped<IDatabaseManagementService, DatabaseManagementService>();

        return services;
    }
}
