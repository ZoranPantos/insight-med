using InsightMed.API.ErrorHandling;

namespace InsightMed.API;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}
