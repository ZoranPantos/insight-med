using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace InsightMed.API.Extensions;

public static class HostingExtensions
{
    public static void LogListeningPort(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var server = app.Services.GetRequiredService<IServer>();
            var addressFeature = server.Features.Get<IServerAddressesFeature>();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            if (addressFeature is not null)
            {
                foreach (var address in addressFeature.Addresses)
                    logger.LogInformation("API is listening on: {Address}", address);
            }
        });
    }
}
