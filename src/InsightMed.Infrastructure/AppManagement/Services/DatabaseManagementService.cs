using InsightMed.Application.AppManagement.Services.Abstractions;
using InsightMed.Application.Common.Abstractions.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InsightMed.Infrastructure.AppManagement.Services;

public sealed class DatabaseManagementService : IDatabaseManagementService
{
    private readonly IAppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public DatabaseManagementService(IAppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task SeedAsync()
    {
        bool isDatabaseEmpty =
            !await _context.Patients.AnyAsync() &&
            !await _context.LabParameters.AnyAsync() &&
            !await _context.LabRequests.AnyAsync() &&
            !await _context.LabReports.AnyAsync() &&
            !await _context.Notifications.AnyAsync() &&
            !await _userManager.Users.AnyAsync();

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