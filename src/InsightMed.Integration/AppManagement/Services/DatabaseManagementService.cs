using InsightMed.Application.AppManagement.Services.Abstractions;
using InsightMed.Integration.Data;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.AppManagement.Services;

public sealed class DatabaseManagementService : IDatabaseManagementService
{
    private readonly AppDbContext _context;

    public DatabaseManagementService(AppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task SeedAsync()
    {
        bool isDatabaseEmpty =
            !await _context.Patients.AnyAsync() &&
            !await _context.LabParameters.AnyAsync() &&
            !await _context.LabRequests.AnyAsync() &&
            !await _context.LabReports.AnyAsync() &&
            !await _context.Notifications.AnyAsync();

        if (!isDatabaseEmpty) return;

        string basePath = Path.GetDirectoryName(typeof(DatabaseManagementService).Assembly.Location)!;
        string filePath = Path.Combine(basePath, "Data/SqlScripts", "SeedData.sql");

        string sql = await File.ReadAllTextAsync(filePath);

        await _context.Database.ExecuteSqlRawAsync(sql);
    }

    public async Task TruncateAsync()
    {
        string basePath = Path.GetDirectoryName(typeof(DatabaseManagementService).Assembly.Location)!;
        string filePath = Path.Combine(basePath, "Data/SqlScripts", "TruncateData.sql");

        string sql = await File.ReadAllTextAsync(filePath);

        await _context.Database.ExecuteSqlRawAsync(sql);
    }
}
