using FluentValidation;
using InsightMed.Application.AppManagement.Commands;
using InsightMed.Application.Common.Behaviors;
using InsightMed.Application.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace InsightMed.Application;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(SeedDatabaseCommand).Assembly);

            config.AddOpenBehavior(typeof(RequestResponseLoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.Configure<MemoryCacheOptions>(configuration.GetSection("MemoryCache"));

        return services;
    }
}
