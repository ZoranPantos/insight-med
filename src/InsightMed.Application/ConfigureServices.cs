using FluentValidation;
using InsightMed.Application.AppManagement.Commands;
using InsightMed.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace InsightMed.Application;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
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

        return services;
    }
}
