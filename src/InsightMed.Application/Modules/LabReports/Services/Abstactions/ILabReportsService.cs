using InsightMed.Domain.Entities;

namespace InsightMed.Application.Modules.LabReports.Services.Abstactions;

public interface ILabReportsService
{
    Task<List<LabReport>> GetAllPagedAsync(int pageNumber, int pageSize);
    Task<List<LabReport>> GetAllByPatientIdAsync(int patientId);
    Task<LabReport?> GetByIdAsync(int id);
    Task AddAsync(LabReport labReport);
    Task<List<LabReport>> SearchByTokensPagedAsync(string[] tokens, int pageNumber, int pageSize);
    Task<int> GetTotalLabReportCountAsync();
}