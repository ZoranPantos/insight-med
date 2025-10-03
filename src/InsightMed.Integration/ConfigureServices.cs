using InsightMed.Application.AppManagement.Services.Abstractions;
using InsightMed.Application.Common.Abstractions;
using InsightMed.Application.LabReports.Services.Abstactions;
using InsightMed.Application.Notifications.Services.Abstractions;
using InsightMed.Application.Patients.Services.Abstractions;
using InsightMed.Infrastructure.AppManagement.Services;
using InsightMed.Infrastructure.Data;
using InsightMed.Infrastructure.LabReports.Services;
using InsightMed.Infrastructure.Notifications.Services;
using InsightMed.Infrastructure.Patients.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InsightMed.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>();
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IDatabaseManagementService, DatabaseManagementService>();
        services.AddScoped<IPatientsService, PatientsService>();
        services.AddScoped<INotificationsService, NotificationsService>();
        services.AddScoped<ILabReportsService, LabReportsService>();

        return services;
    }
}
