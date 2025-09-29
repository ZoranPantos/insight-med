using InsightMed.Application.AppManagement.Services.Abstractions;
using InsightMed.Integration.Data;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.AppManagement.Services;

public class DatabaseManagementService : IDatabaseManagementService
{
    private readonly AppDbContext _context;

    public DatabaseManagementService(AppDbContext context) =>
        _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task Seed()
    {
        string basePath = Path.GetDirectoryName(typeof(DatabaseManagementService).Assembly.Location)!;
        string filePath = Path.Combine(basePath, "Data", "SeedData.sql");

        string sql = await File.ReadAllTextAsync(filePath);

        await _context.Database.ExecuteSqlRawAsync(sql);
    }
}
