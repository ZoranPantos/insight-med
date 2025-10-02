namespace InsightMed.Application.AppManagement.Services.Abstractions;

public interface IDatabaseManagementService
{
    Task SeedAsync();
    Task TruncateAsync();
}
