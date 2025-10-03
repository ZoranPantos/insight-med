using InsightMed.Domain.Entities;

namespace InsightMed.Application.LabReports.Services.Abstactions;

public interface ILabReportsService
{
    Task<List<LabReport>> GetAllAsync();
}
