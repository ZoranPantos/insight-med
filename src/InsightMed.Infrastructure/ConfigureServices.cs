using InsightMed.Application.AppManagement.Services.Abstractions;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Common.Abstractions.Messaging;
using InsightMed.Application.LabParameters.Services.Abstractions;
using InsightMed.Application.LabReports.Services.Abstactions;
using InsightMed.Application.LabRequests.Services.Abstractions;
using InsightMed.Application.Notifications.Services.Abstractions;
using InsightMed.Application.Patients.Services.Abstractions;
using InsightMed.Infrastructure.AppManagement.Services;
using InsightMed.Infrastructure.Data;
using InsightMed.Infrastructure.HostedServices;
using InsightMed.Infrastructure.LabParameters.Services;
using InsightMed.Infrastructure.LabReports.Services;
using InsightMed.Infrastructure.LabRequests.Services;
using InsightMed.Infrastructure.Messaging;
using InsightMed.Infrastructure.Notifications.Services;
using InsightMed.Infrastructure.Options;
using InsightMed.Infrastructure.Patients.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InsightMed.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>();
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IDatabaseManagementService, DatabaseManagementService>();
        services.AddScoped<IPatientsService, PatientsService>();
        services.AddScoped<INotificationsService, NotificationsService>();
        services.AddScoped<ILabReportsService, LabReportsService>();
        services.AddScoped<ILabRequestsService, LabRequestsService>();
        services.AddScoped<ILabParametersService, LabParametersService>();

        services.AddSingleton<RabbitMqRpcClient>();
        services.AddSingleton<ILabRpcClient>(sp => sp.GetRequiredService<RabbitMqRpcClient>());

        services.AddHostedService<RabbitMqRpcClientHostedService>();

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));

        return services;
    }
}
