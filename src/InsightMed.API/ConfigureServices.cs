using InsightMed.API.ErrorHandling;
using InsightMed.Application.AppManagement.Commands;

namespace InsightMed.API;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(SeedDatabaseCommand).Assembly);
        });

        return services;
    }
}
