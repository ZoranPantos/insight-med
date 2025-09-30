using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace InsightMed.Application;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
