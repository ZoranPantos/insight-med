using InsightMed.API.ErrorHandling;
using InsightMed.API.Services;
using InsightMed.Application.Modules.Notifications.Services.Abstractions;

namespace InsightMed.API.Configurations;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureApi(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        services.AddControllers();
        services.AddOpenApi();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddSignalR();

        services.AddScoped<INotificationsNotifierService, NotificationsNotifierService>();

        return services;
    }
}