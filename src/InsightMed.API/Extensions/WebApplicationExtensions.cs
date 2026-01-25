using InsightMed.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migration step completed");
        }
        catch (SqlException ex) when (ex.Number == 1801)
        {
            logger.LogWarning(
                ex,
                "Database {Database} already exists. Skipping creation",
                dbContext.Database.GetDbConnection().Database);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in database migration step");
            throw;
        }
    }
}