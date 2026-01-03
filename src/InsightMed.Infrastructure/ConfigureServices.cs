using InsightMed.Application.AppManagement.Services.Abstractions;
using InsightMed.Application.Auth.Services.Abstractions;
using InsightMed.Application.Common.Abstractions.Data;
using InsightMed.Application.Common.Abstractions.Messaging;
using InsightMed.Application.Modules.LabParameters.Services.Abstractions;
using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using InsightMed.Application.Modules.LabRequests.Services.Abstractions;
using InsightMed.Application.Modules.Notifications.Services.Abstractions;
using InsightMed.Application.Modules.Patients.Services.Abstractions;
using InsightMed.Infrastructure.AppManagement.Services;
using InsightMed.Infrastructure.Auth.Services;
using InsightMed.Infrastructure.Data;
using InsightMed.Infrastructure.HostedServices;
using InsightMed.Infrastructure.Messaging;
using InsightMed.Infrastructure.Modules.LabParameters.Services;
using InsightMed.Infrastructure.Modules.LabReports.Services;
using InsightMed.Infrastructure.Modules.LabRequests.Services;
using InsightMed.Infrastructure.Modules.Notifications.Services;
using InsightMed.Infrastructure.Modules.Patients.Services;
using InsightMed.Infrastructure.Options;
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
        services.AddScoped<IAuthService, AuthService>();

        services.AddSingleton<RabbitMqRpcClient>();
        services.AddSingleton<ILabRpcClient>(sp => sp.GetRequiredService<RabbitMqRpcClient>());

        services.AddHostedService<RabbitMqRpcClientHostedService>();

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));

        return services;
    }
}
